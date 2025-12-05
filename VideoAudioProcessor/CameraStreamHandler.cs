using System.Net.Sockets;
using System.Text;
using System.Security.Cryptography;
using NLog;
using NAudio.Wave;

namespace VideoAudioProcessor;

public class CameraStreamHandler
{
    private readonly Logger _logs;
    private readonly Config _config;
    
    private List<byte> _vframe = [];
    private List<byte> _aframe = [];
    
    // ADPCM decoder state (maintain across frames for continuous stream decoding)
    private int _adpcmPredictor = 0;
    private int _adpcmStepIndex = 0;
    private bool _adpcmStateInitialized = false;
    private int _adpcmNibbleFlag = 0;
    private int _adpcmCurrentByte = 0;
    
    // Network connection
    private TcpClient? _streamClient;
    private NetworkStream? _networkStream;
    private bool _isStreaming = false;

    public event EventHandler<byte[]>? VideoFrameReceived;
    public event EventHandler<byte[]>? AudioFrameReceived;

    // Reuse v380stream login classes
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
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);
            writer.Write(Command);
            writer.Write(Unknown1);
            writer.Write(Unknown2);
            writer.Write(Unknown3);
            writer.Write(DeviceId);
            writer.Write(HostDateTime);
            writer.Write(UserName);
            writer.Write(Password);
            return ms.ToArray();
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
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);
            writer.Write(Command);
            writer.Write(DeviceId);
            writer.Write(Unknown1);
            writer.Write(MaybeFps);
            writer.Write(AuthTicket);
            writer.Write(Unknown3);
            writer.Write(Unknown4);
            writer.Write(Resolution);
            writer.Write(Unknown6);
            return ms.ToArray();
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
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);
            writer.Write(Command);
            writer.Write(Unknown1);
            return ms.ToArray();
        }
    }

    public CameraStreamHandler(Logger log, Config config)
    {
        _logs = log;
        _config = config;
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

    private byte[] GeneratePassword(string password)
    {
        int nBlockLen = 16;
        byte[] randomKey = GenerateRandomPrintable(nBlockLen);
        byte[] staticKey = Encoding.ASCII.GetBytes("macrovideo+*#!^@".PadRight(nBlockLen, '\0'));

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
        byte[] pad = new byte[nBlockLen - (password.Length % nBlockLen)];
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

    public async Task<bool> ConnectAsync()
    {
        if (_config.CameraIP == null || _config.CameraID == null || 
            _config.CameraUserName == null || _config.CameraPassword == null)
        {
            _logs.Error("Camera configuration incomplete");
            return false;
        }

        try
        {
            using var socketAuth = new TcpClient(_config.CameraIP, _config.CameraPort);
            using var networkStream = socketAuth.GetStream();

            var loginReqData = new LoginRequest
            {
                Command = 1167,
                DeviceId = UInt32.Parse(_config.CameraID),
                Unknown1 = 1022,
                Unknown2 = 2,
                Unknown3 = 1,
            };
            Buffer.BlockCopy(Encoding.ASCII.GetBytes(_config.CameraUserName), 0, loginReqData.UserName, 0,
                _config.CameraUserName.Length);
            var password = GeneratePassword(_config.CameraPassword);
            Buffer.BlockCopy(password, 0, loginReqData.Password, 0, password.Length);

            var loginRequest = loginReqData.Serialize();
            await networkStream.WriteAsync(loginRequest);

            var response = new byte[256];
            var bytesRead = await networkStream.ReadAsync(response);
            if (bytesRead <= 0)
            {
                return false;
            }

            var loginResp = LoginResponse.Deserialize(response);
            if (loginResp.Command == 1168 && loginResp.LoginResult == 1001)
            {
                _logs.Info("Camera authenticated");
                return true;
            }
        }
        catch (Exception ex)
        {
            _logs.Error(ex, "Failed to authenticate with camera");
        }

        return false;
    }

    public async Task StartStreamingAsync()
    {
        if (_config.CameraIP == null || _config.CameraID == null) return;

        // Authenticate first
        UInt32 authToken = 0;
        int bytesRead = 0;
        using (var socketAuth = new TcpClient(_config.CameraIP, _config.CameraPort))
        {
            using var authStream = socketAuth.GetStream();

            var loginReqData = new LoginRequest
        {
            Command = 1167,
            DeviceId = UInt32.Parse(_config.CameraID),
            Unknown1 = 1022,
            Unknown2 = 2,
            Unknown3 = 1,
        };
            if (_config.CameraUserName != null)
            {
                Buffer.BlockCopy(Encoding.ASCII.GetBytes(_config.CameraUserName), 0, loginReqData.UserName, 0,
                    _config.CameraUserName.Length);
            }
            if (_config.CameraPassword != null)
            {
                var password = GeneratePassword(_config.CameraPassword);
                Buffer.BlockCopy(password, 0, loginReqData.Password, 0, password.Length);
            }

            var loginRequest = loginReqData.Serialize();
            await authStream.WriteAsync(loginRequest);

            var response = new byte[256];
            bytesRead = await authStream.ReadAsync(response);
        if (bytesRead <= 0)
        {
            _logs.Error("Failed to get login response");
            return;
        }

            var loginResp = LoginResponse.Deserialize(response);
            if (loginResp.Command != 1168 || loginResp.LoginResult != 1001)
            {
                _logs.Error($"Camera login failed: {loginResp.LoginResult}");
                return;
            }

            authToken = loginResp.AuthTicket;
        }

        // Now start streaming
        _streamClient = new TcpClient(_config.CameraIP, _config.CameraPort);
        _networkStream = _streamClient.GetStream();
        _networkStream.WriteTimeout = 5000;

        var buff = new byte[256];
        var streamLoginLanReq = new StreamLoginLanRequest
        {
            Command = 301,
            DeviceId = UInt32.Parse(_config.CameraID),
            Unknown1 = 0,
            MaybeFps = 20,
            AuthTicket = authToken,
            Unknown3 = 0,
            Unknown4 = 4096 + 1,
            Resolution = 1,
            Unknown6 = 0
        };
        streamLoginLanReq.Serialize().CopyTo(buff, 0);
        await _networkStream.WriteAsync(buff, 0, buff.Length);
        Array.Clear(buff);

        bytesRead = await _networkStream.ReadAsync(buff, 0, buff.Length);
        if (bytesRead == 0)
        {
            _logs.Error("Error starting stream");
            StopStreaming();
            return;
        }

        var streamLoginResp = StreamLogin301Response.Deserialize(buff);
        if (streamLoginResp.Command != 401)
        {
            _logs.Warn($"Unexpected stream login response: {streamLoginResp.Command}");
        }

        Array.Clear(buff);
        var streamStartReq = new StreamStartRequest
        {
            Command = 303,
            Unknown1 = (UInt32)streamLoginResp.V21
        };
        streamStartReq.Serialize().CopyTo(buff, 0);
        await _networkStream.WriteAsync(buff, 0, buff.Length);

        _isStreaming = true;
        _logs.Info("Streaming started");

        // Start receiving frames
        _ = Task.Run(async () => await ReceiveFramesAsync());
    }

    private async Task ReceiveFramesAsync()
    {
        if (_networkStream == null) return;

        var header = new byte[12];
        _networkStream.ReadTimeout = System.Threading.Timeout.Infinite;

        while (_isStreaming)
        {
            int totalBytesRead = 0;
            while (totalBytesRead < 12 && _networkStream != null && _isStreaming)
            {
                try
                {
                    int bytesRead = await _networkStream.ReadAsync(header, totalBytesRead, 12 - totalBytesRead);
                    if (bytesRead == 0)
                    {
                        _logs.Trace("Connection closed by remote host");
                        break;
                    }
                    totalBytesRead += bytesRead;
                }
                catch (ObjectDisposedException)
                {
                    _logs.Trace("Network stream was closed");
                    return;
                }
                catch (IOException)
                {
                    _logs.Trace("Network connection closed");
                    return;
                }
            }

            if (totalBytesRead < 12) break;

            try
            {
                await HandleStreamFrameAsync(header);
            }
            catch (Exception ex)
            {
                _logs.Warn(ex, "Error processing stream frame");
            }
        }

        StopStreaming();
    }

    private async Task HandleStreamFrameAsync(byte[] hdr)
    {
        if (_networkStream == null) return;

        ushort totalFrame = BitConverter.ToUInt16(hdr, 3);
        ushort curFrame = BitConverter.ToUInt16(hdr, 5);
        ushort len = BitConverter.ToUInt16(hdr, 7);
        byte type = hdr[1];

        if (len > 500 || totalFrame == 0 || curFrame > totalFrame)
        {
            return;
        }

        byte[] frameData = new byte[len];
        int n = 0;
        while (n < len && _networkStream != null)
        {
            try
            {
                int read = await _networkStream.ReadAsync(frameData, n, len - n);
                if (read == 0)
                {
                    return;
                }
                n += read;
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (IOException)
            {
                return;
            }
        }

        switch (type)
        {
            case 0x00: // I-frame
            case 0x01: // P-frame
                _vframe.AddRange(frameData);
                if (curFrame == totalFrame - 1)
                {
                    var videoFrame = _vframe.ToArray();
                    _vframe.Clear();
                    VideoFrameReceived?.Invoke(this, videoFrame);
                }
                break;

            case 0x16: // Audio frame
                _aframe.AddRange(frameData);
                if (curFrame == totalFrame - 1)
                {
                    var fullFrame = _aframe.ToArray();
                    _aframe.Clear();

                    // Decode ADPCM to PCM (reuse v380stream logic)
                    const int skipBytes = 20;
                    if (fullFrame.Length > skipBytes)
                    {
                        byte[] adpcmData = new byte[fullFrame.Length - skipBytes];
                        Array.Copy(fullFrame, skipBytes, adpcmData, 0, adpcmData.Length);

                        byte[] pcmData;
                        if (!_adpcmStateInitialized)
                        {
                            // First frame: find header
                            int headerOffset = -1;
                            if (adpcmData.Length >= 4)
                            {
                                int step0 = adpcmData[2];
                                if (step0 >= 0 && step0 <= 88)
                                {
                                    headerOffset = 0;
                                }
                                else if (adpcmData.Length >= 8)
                                {
                                    int step4 = adpcmData[6];
                                    if (step4 >= 0 && step4 <= 88)
                                    {
                                        headerOffset = 4;
                                    }
                                }
                            }

                            if (headerOffset >= 0)
                            {
                                byte[] headerData = new byte[adpcmData.Length - headerOffset];
                                Array.Copy(adpcmData, headerOffset, headerData, 0, headerData.Length);

                                _adpcmPredictor = headerData[0] | (headerData[1] << 8);
                                if (_adpcmPredictor > 32767) _adpcmPredictor -= 65536;
                                _adpcmStepIndex = headerData[2];
                                if (_adpcmStepIndex > 88) _adpcmStepIndex = 88;
                                _adpcmStateInitialized = true;

                                int adpcmBytes = headerData.Length - 4;
                                int outputSamples = 2 + (adpcmBytes * 2);
                                pcmData = ImaAdpcmDecoder.Decode(headerData, outputSamples, false, 0, 0,
                                    out _adpcmPredictor, out _adpcmStepIndex,
                                    ref _adpcmNibbleFlag, ref _adpcmCurrentByte);
                            }
                            else
                            {
                                _adpcmPredictor = 0;
                                _adpcmStepIndex = 0;
                                _adpcmStateInitialized = true;
                                int outputSamples = adpcmData.Length * 2;
                                pcmData = ImaAdpcmDecoder.Decode(adpcmData, outputSamples, true,
                                    _adpcmPredictor, _adpcmStepIndex,
                                    out _adpcmPredictor, out _adpcmStepIndex,
                                    ref _adpcmNibbleFlag, ref _adpcmCurrentByte);
                            }
                        }
                        else
                        {
                            // Subsequent frames: continue decoding
                            int outputSamples = adpcmData.Length * 2;
                            pcmData = ImaAdpcmDecoder.Decode(adpcmData, outputSamples, true,
                                _adpcmPredictor, _adpcmStepIndex,
                                out _adpcmPredictor, out _adpcmStepIndex,
                                ref _adpcmNibbleFlag, ref _adpcmCurrentByte);
                        }

                        if (pcmData.Length > 0)
                        {
                            AudioFrameReceived?.Invoke(this, pcmData);
                        }
                    }
                }
                break;
        }
    }

    public void StopStreaming()
    {
        _isStreaming = false;
        try
        {
            _networkStream?.Close();
            _streamClient?.Close();
        }
        catch { }
    }
}

