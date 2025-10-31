using System.Net.Sockets;
using System.Text;
using System.Security.Cryptography;
using NLog;
using NAudio.Wave;
using System.Threading.Channels;

namespace v380stream;

class ConnectionHanler
{
    private readonly Logger _logs;
    private readonly Config _conf;

  private List<byte> vframe = [];
    private List<byte> aframe = [];

    // File streams for output (matching C++ implementation)
    private FileStream? _videoFile;
    private FileStream? _audioFile;
    
    // Network connection for streaming (needed to interrupt blocking reads on Ctrl+C)
    private TcpClient? _streamClient;
    private NetworkStream? _networkStream;

    // Define data classes or structs for the requests and responses
    private class LoginRequest
    {
        public UInt32 Command;
        public UInt32 Unknown1;
        public byte Unknown2;
        public UInt32 Unknown3;
        public UInt32 DeviceId;
        public byte[] HostDateTime = new byte[32];
        public byte[] UserName = new byte[32];
        public byte[] Password = new byte[32];

        public byte[] Serialize()
        {
            using var memoryStream = new MemoryStream();
            using var writer = new BinaryWriter(memoryStream);

            writer.Write(Command);
            writer.Write(Unknown1);
            writer.Write(Unknown2);
            writer.Write(Unknown3);
            writer.Write(DeviceId);

            writer.Write(HostDateTime);
            writer.Write(UserName);
            writer.Write(Password);

            return memoryStream.ToArray();
        }
    }

    private class LoginResponse
    {
        public Int32 Command;
        public Int32 LoginResult;
        public Int32 ResultValue;
        public byte Version;
        public UInt32 AuthTicket;
        public UInt32 Session;
        public byte DeviceType;
        public byte CamType;
        public UInt16 VendorId;
        public UInt16 IsDomainExists;
        public byte[] Domain = new byte[32];
        public Int32 RecDevId;
        public byte NChannels;
        public byte NAudioPri;
        public byte NVideoPri;
        public byte NSpeaker;
        public byte NPtzPri;
        public byte NReversePri;
        public byte NPtzXPri;
        public byte NPtzXCount;
        public byte[] Settings = new byte[32];
        public UInt16 PanoX;
        public UInt16 PanoY;
        public UInt16 PanoRad;
        public UInt32 Unknown1;
        public byte CanUpdateDevice;

        public static LoginResponse Deserialize(byte[] bytes)
        {
            using var ms = new MemoryStream(bytes);
            using var br = new BinaryReader(ms);

            var response = new LoginResponse();
            response.Command = br.ReadInt32();
            response.LoginResult = br.ReadInt32();
            response.ResultValue = br.ReadInt32();
            response.Version = br.ReadByte();
            response.AuthTicket = br.ReadUInt32();
            response.Session = br.ReadUInt32();
            response.DeviceType = br.ReadByte();
            response.CamType = br.ReadByte();
            response.VendorId = br.ReadUInt16();
            response.IsDomainExists = br.ReadUInt16();
            response.Domain = br.ReadBytes(response.Domain.Length);
            response.RecDevId = br.ReadInt32();
            response.NChannels = br.ReadByte();
            response.NAudioPri = br.ReadByte();
            response.NVideoPri = br.ReadByte();
            response.NSpeaker = br.ReadByte();
            response.NPtzPri = br.ReadByte();
            response.NReversePri = br.ReadByte();
            response.NPtzXPri = br.ReadByte();
            response.NPtzXCount = br.ReadByte();
            response.Settings = br.ReadBytes(response.Settings.Length);
            response.PanoX = br.ReadUInt16();
            response.PanoY = br.ReadUInt16();
            response.PanoRad = br.ReadUInt16();
            response.Unknown1 = br.ReadUInt32();
            response.CanUpdateDevice = br.ReadByte();

            return response;
        }
    }

    private class StreamLoginLanRequest
    {
        public UInt32 Command;
        public UInt32 DeviceId;
        public UInt32 Unknown1;
        public UInt16 MaybeFps;
        public UInt32 AuthTicket;
        public UInt32 Unknown3;
        public UInt32 Unknown4;
        public UInt32 Resolution;
        public UInt32 Unknown6;

        public byte[] Serialize()
        {
            using var memoryStream = new MemoryStream();
            using var writer = new BinaryWriter(memoryStream);

            writer.Write(Command);
            writer.Write(DeviceId);
            writer.Write(Unknown1);
            writer.Write(MaybeFps);
            writer.Write(AuthTicket);
            writer.Write(Unknown3);
            writer.Write(Unknown4);
            writer.Write(Resolution);
            writer.Write(Unknown6);

            return memoryStream.ToArray();
        }
    }

    private class StreamLogin301Response
    {
        public Int32 Command;
        public Int32 V21;
        public UInt32 MaybeFps;
        public UInt32 Width;
        public UInt32 Height;

        public static StreamLogin301Response Deserialize(byte[] bytes)
        {
            using var ms = new MemoryStream(bytes);
            using var br = new BinaryReader(ms);

            var response = new StreamLogin301Response();
            response.Command = br.ReadInt32();
            response.V21 = br.ReadInt32();
            response.MaybeFps = br.ReadUInt32();
            response.Width = br.ReadUInt32();
            response.Height = br.ReadUInt32();

            return response;
        }
    }

    private class StreamStartRequest
    {
        public Int32 Command;
        public UInt32 Unknown1;

        public byte[] Serialize()
        {
            using var memoryStream = new MemoryStream();
            using var writer = new BinaryWriter(memoryStream);

            writer.Write(Command);
            writer.Write(Unknown1);

            return memoryStream.ToArray();
        }
    }

    public ConnectionHanler(Logger log, Config conf)
    {
        _logs = log;
      _conf = conf;
    }

    private byte[] GenerateRandomPrintable(int length)
    {
        const string set = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()_+-=";
        Random random = new();
        byte[] result = new byte[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = (byte)set[random.Next(set.Length)];
        }

        return result;
    }

    private byte[] EncryptAesEcb(byte[] input, byte[] key)
    {
        using var aes = Aes.Create();
        aes.KeySize = 128;
        aes.BlockSize = 128;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.None;
        aes.Key = key;
        aes.IV = new byte[16]; // zero IV for ECB simulation

        using ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        return encryptor.TransformFinalBlock(input, 0, input.Length);
    }

    private byte[] GeneratePassword(string password)
    {
        int nBlockLen = 16;
        byte[] randomKey = GenerateRandomPrintable(nBlockLen);
        byte[] staticKey = Encoding.ASCII.GetBytes("macrovideo+*#!^@".PadRight(nBlockLen, '\0'));
        byte[] pad = new byte[nBlockLen - (password.Length % nBlockLen)];

        using var aes1 = Aes.Create();
        using var aes2 = Aes.Create();

        aes1.Key = staticKey;
        aes1.Mode = CipherMode.ECB;
        aes1.Padding = PaddingMode.None;

        aes2.Key = randomKey;
        aes2.Mode = CipherMode.ECB;
        aes2.Padding = PaddingMode.None;

        var encryptor1 = aes1.CreateEncryptor();
        var encryptor2 = aes2.CreateEncryptor();

        byte[] passwordBytes = Encoding.ASCII.GetBytes(password);
        byte[] catBuff = new byte[passwordBytes.Length + pad.Length];
        Buffer.BlockCopy(passwordBytes, 0, catBuff, 0, passwordBytes.Length);
        Buffer.BlockCopy(pad, 0, catBuff, passwordBytes.Length, pad.Length);

        byte[] res1 = encryptor1.TransformFinalBlock(catBuff, 0, catBuff.Length);
        byte[] res2 = encryptor2.TransformFinalBlock(res1, 0, res1.Length);

        byte[] result = new byte[randomKey.Length + res2.Length];
        Buffer.BlockCopy(randomKey, 0, result, 0, randomKey.Length);
        Buffer.BlockCopy(res2, 0, result, randomKey.Length, res2.Length);

        return result;
    }

    public UInt32 Authorise()
    {
        if (_conf.IP != null)
        {
            using var socketAuth = new TcpClient(_conf.IP, _conf.Port);
            using var networkStream = socketAuth.GetStream();

            var loginRequest = new byte[256];
            if (_conf is { ID: not null, UserName: not null, Password: not null })
            {
                var loginReqData = new LoginRequest
                {
                    Command = 1167,
                    DeviceId = UInt32.Parse(_conf.ID),
                    Unknown1 = 1022,
                    Unknown2 = 2,
                    Unknown3 = 1,
                };
                Buffer.BlockCopy(Encoding.ASCII.GetBytes(_conf.UserName), 0, loginReqData.UserName, 0,
                    _conf.UserName.Length);
                var password = GeneratePassword(_conf.Password);
                Buffer.BlockCopy(password, 0, loginReqData.Password, 0, password.Length);

                // Pack the loginReqData into loginRequest buffer
                loginReqData.Serialize().CopyTo(loginRequest, 0);
            }

            networkStream.Write(loginRequest, 0, loginRequest.Length);

            var response = new byte[256];
            var bytesRead = networkStream.Read(response, 0, response.Length);
            if (bytesRead <= 0)
            {
                return 0;
            }

            var loginResp = LoginResponse.Deserialize(response);

            if (loginResp.Command == 1168)
            {
                switch (loginResp.LoginResult)
                {
                    case 1001:
                        _logs.Trace("Camera logged in");
                        return loginResp.AuthTicket;
                    case 1011:
                        _logs.Trace("Invalid username");
                        break;
                    case 1012:
                        _logs.Trace("Invalid password");
                        break;
                    case 1018:
                        _logs.Trace("Invalid device id");
                        break;
                }
            }
        }

        return 0;
    }

    public void StartStreaming(UInt32 authToken)
    {
   var buff = new byte[256];
        if (_conf is { IP: not null, ID: not null })
      {
            // Open output files (exactly like C++ implementation)
   try
            {
         _videoFile = new FileStream("video.h264", FileMode.Create, FileAccess.Write);
      _audioFile = new FileStream("audio.raw", FileMode.Create, FileAccess.Write);
                _logs.Info("Created output files: video.h264 and audio.raw");
            }
          catch (Exception ex)
            {
                _logs.Error(ex, "Failed to create output files");
      return;
  }

      _streamClient = new TcpClient(_conf.IP, _conf.Port);
   _networkStream = _streamClient.GetStream();
            _networkStream.WriteTimeout = 5000;

  // ...existing stream login code...

         var streamLoginLanReq = new StreamLoginLanRequest
            {
      Command = 301,
          DeviceId = UInt32.Parse(_conf.ID),
     Unknown1 = 0,
              MaybeFps = 20,
   AuthTicket = authToken,
     Unknown3 = 0,
      Unknown4 = 4096 + 1,
          Resolution = 1,
           Unknown6 = 0
      };
            streamLoginLanReq.Serialize().CopyTo(buff, 0);
            _networkStream.Write(buff, 0, buff.Length);
            Array.Clear(buff);

            int bytesRead = _networkStream.Read(buff, 0, buff.Length);
            if (bytesRead == 0)
     {
    _logs.Trace("Error starting stream");
         StopStreaming();
              return;
          }

  var streamLoginResp = StreamLogin301Response.Deserialize(buff);
      if (streamLoginResp.Command != 401)
 {
     _logs.Trace("Login response: expected 401, got {streamLoginResp.Command}");
            }

            if (streamLoginResp.V21 == -11 || streamLoginResp.V21 == -12)
            {
         _logs.Trace("Login response: unsupported {streamLoginResp.V21}, continuing");
            }
else if (streamLoginResp.V21 != 402 && streamLoginResp.V21 != 1001)
            {
          _logs.Trace("Login response: unsupported {streamLoginResp.V21}");
          }

            Array.Clear(buff);

            //start stream
            var streamStartReq = new StreamStartRequest
            {
  Command = 303,
              Unknown1 = (UInt32)streamLoginResp.V21
            };
            streamStartReq.Serialize().CopyTo(buff, 0);
            _networkStream.Write(buff, 0, buff.Length);

         //receive bytes
     var header = new byte[12];
            _networkStream.ReadTimeout = System.Threading.Timeout.Infinite; // No timeout for blocking reads
            while (true)
      {
      int totalBytesRead = 0;
      // Read header in loop to handle partial reads
      while (totalBytesRead < 12 && _networkStream != null)
      {
          try
          {
              bytesRead = _networkStream.Read(header, totalBytesRead, 12 - totalBytesRead);
              if (bytesRead == 0)
              {
                  // Connection closed
                  _logs.Trace("Connection closed by remote host");
                  break;
              }
              totalBytesRead += bytesRead;
          }
          catch (ObjectDisposedException)
          {
              // Stream was closed (e.g., by StopStreaming on Ctrl+C)
              _logs.Trace("Network stream was closed, exiting streaming loop");
              break;
          }
          catch (IOException)
          {
              // Connection was closed
              _logs.Trace("Network connection closed");
              break;
          }
      }
      
      if (totalBytesRead < 12)
      {
          // Connection closed, exit streaming loop
          _logs.Warn($"Incomplete header received ({totalBytesRead} bytes, expected 12). Connection closed.");
          break;
      }
      
      // Continue processing even if there are issues - don't stop streaming on errors

           switch (header[0])
  {
         case 0x7f:
         try
         {
             HandleStream(header, _networkStream!);
         }
         catch (Exception ex)
         {
             // Log error but continue streaming - don't stop on frame processing errors
             _logs.Warn(ex, $"Error processing stream frame (type {header[1]}): {ex.Message}. Continuing...");
         }
         break;

             case 0x1f:
 _logs.Trace("Unparsed 0x1f data");
        break;

                    case 0x6f:
            Thread.Sleep(20);
   break;

          default:
    _logs.Trace("Unknown data: " + header[0] + " (continuing stream)");
           // Continue streaming even with unknown headers
           break;
        }
            }
       
            // Clean up when streaming stops
    StopStreaming();
  }
    }

    private void HandleStream(byte[] hdr, NetworkStream networkStream)
    {
        // Read the total frame count, current frame number, and length from the header
    ushort totalFrame = BitConverter.ToUInt16(hdr, 3);
    ushort curFrame = BitConverter.ToUInt16(hdr, 5);
 ushort len = BitConverter.ToUInt16(hdr, 7);
        byte type = hdr[1];

    if (len > 500 || totalFrame == 0 || curFrame > totalFrame)
      {
        _logs.Trace("Sanity check failed, should not happen");
     return;
        }

        // Receive the frame data from the server
        byte[] frameData = new byte[len];
     int n = 0;
    while (n < len)
  {
          try
          {
              int read = networkStream.Read(frameData, n, len - n);
              if (read == 0)
              {
                  // Connection closed while reading frame data
                  _logs.Warn($"Connection closed while reading frame data (read {n} of {len} bytes). Skipping frame.");
                  return; // Skip this frame and continue with next header
              }
              n += read;
          }
          catch (ObjectDisposedException)
          {
              // Stream was closed (e.g., by StopStreaming on Ctrl+C)
              _logs.Trace("Network stream closed while reading frame data");
              return;
          }
          catch (IOException)
          {
              // Connection was closed
              _logs.Trace("Network connection closed while reading frame data");
              return;
          }
        }

   // Handle the received frame based on its type (I-Frame or P-Frame)
        switch (type)
        {
            // Handle video
       case 0x00: //i-frame
        case 0x01: //p-frame
          vframe.AddRange(frameData);
       if (curFrame == totalFrame - 1)
 {
         // Write to video.h264 (matching C++ implementation)
           try
  {
         _videoFile?.Write(vframe.ToArray(), 0, vframe.Count);
     _videoFile?.Flush();
             _logs.Trace($"Wrote video frame: {vframe.Count} bytes");
        }
               catch (Exception ex)
          {
          _logs.Error(ex, "Failed to write video frame");
       }
            vframe.Clear();
      }
      break;

          case 0x16:
          // Audio
       // Node.js (stream.ts line 134): skips 20 bytes -> slice(20)
       // C++ FLV (FlvStream.cpp line 493): skips 18 bytes -> begin + 18  
       // Raw output (v380.cpp line 501): writes full frames (no skip)
       // Current: writing full frames for raw output, conversion tools should skip headers
       // Node.js FFmpeg: -f s16le -ar 8000 -acodec adpcm_ima_ws -ac 1 (with 20-byte skip)
           aframe.AddRange(frameData);
        if (curFrame == totalFrame - 1)
       {
 // Write to audio.raw - write full frame matching C++ raw output (v380.cpp line 501)
         try
    {
   var fullFrame = aframe.ToArray();
   // C++ raw audio output (v380.cpp line 501): fwrite(aframe.data() + 0, 1, aframe.size() - 0, stdout)
   // Writes full frames without any header skipping for raw output
   if (fullFrame.Length > 0)
   {
       _audioFile?.Write(fullFrame, 0, fullFrame.Length);
       _audioFile?.Flush();
       _logs.Trace($"Wrote audio frame: {fullFrame.Length} bytes (full frame, matching C++ raw output)");
   }
}
            catch (Exception ex)
   {
          _logs.Error(ex, "Failed to write audio frame");
     }
     
           aframe.Clear();
         }
              break;
        }
    }

    private void PlayAudio()
    {
        var audioSample = new MemoryStream(aframe.ToArray());
        var fileFormat = new ImaAdpcmWaveFormat(8000, 1, 16);
        var provider = new RawSourceWaveStream(audioSample, fileFormat);

        var stream = WaveFormatConversionStream.CreatePcmStream(provider);

      using var outputDevice = new WaveOutEvent();
     outputDevice.Init(stream);
        outputDevice.Play();
    }

    public void StopStreaming()
    {
        _logs.Info("Stopping stream and closing files");
        
   try
    {
            _videoFile?.Close();
   _videoFile?.Dispose();
            _videoFile = null;
        }
        catch (Exception ex)
        {
            _logs.Error(ex, "Error closing video file");
        }

        try
        {
        _audioFile?.Close();
       _audioFile?.Dispose();
   _audioFile = null;
        }
    catch (Exception ex)
     {
        _logs.Error(ex, "Error closing audio file");
    }
    
    // Close network connection to interrupt any blocking reads
    try
    {
        _networkStream?.Close();
        _networkStream?.Dispose();
        _networkStream = null;
        
        _streamClient?.Close();
        _streamClient?.Dispose();
        _streamClient = null;
    }
    catch (Exception ex)
    {
        _logs.Error(ex, "Error closing network connection");
    }
    }
}