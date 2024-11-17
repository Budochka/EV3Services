using NAudio.Wave;
using System;
using System.IO;

namespace v380stream;

public class FlvStream
{
    private bool _enableAudio = true;
    private bool _exit = false;
    private int _audioDts = 0;
    private int _audioPts = 0;
    private int _lastTimestamp = 0;    
    
    public void WriteAudio(byte[] packet)
    {
        if (!_enableAudio || _exit || packet.Length == 0)
        {
            return;
        }
        // Buffer and Data Preparation
        using (MemoryStream buffer = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(buffer))
        {
            // ADPCM Decoding and Resampling
            byte[] pcmData = DecodeAdpcm(packet);
            byte[] resampledData = ResamplePcm(pcmData, 8000, 11025);
            // Timestamp Calculation
            int timestamp = CalculateTimestamp(packet);
            // FLV Tag and Audio Data Writing
            WriteFlvTag(writer, resampledData.Length, timestamp);
            WriteFlvAudioData(writer, resampledData);
            // Finalizing and Writing to Output
            WritePreviousTagSize(writer, resampledData.Length);
            buffer.WriteTo(Console.OpenStandardOutput());
        }
    }
    
    private byte[] DecodeAdpcm(byte[] adpcmData)
    {
        using (MemoryStream ms = new MemoryStream(adpcmData))
        using (WaveStream waveStream = new WaveFileReader(ms))
        using (WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(waveStream))
        using (MemoryStream pcmMs = new MemoryStream())
        {
            pcmStream.CopyTo(pcmMs);
            return pcmMs.ToArray();
        }
    }
    
    private byte[] ResamplePcm(byte[] pcmData, int inputRate, int outputRate)
    {
        using (WaveFormatConversionStream resampler = new WaveFormatConversionStream(
            new WaveFormat(outputRate, 16, 1),
            new RawSourceWaveStream(new MemoryStream(pcmData), new WaveFormat(inputRate, 16, 1))))
        using (MemoryStream resampledMs = new MemoryStream())
        {
            resampler.CopyTo(resampledMs);
            return resampledMs.ToArray();
        }
    }
    
    private int CalculateTimestamp(byte[] packet)
    {
        // Implement timestamp calculation logic here
        return 0;
    }
    
    private void WriteFlvTag(BinaryWriter writer, int dataSize, int timestamp)
    {
        // Implement FLV tag writing logic here
    }
    
    private void WriteFlvAudioData(BinaryWriter writer, byte[] audioData)
    {
        // Implement FLV audio data writing logic here
    }
    
    private void WritePreviousTagSize(BinaryWriter writer, int dataSize)
    {
        writer.Write(dataSize + 11); // Example size calculation
    }
}