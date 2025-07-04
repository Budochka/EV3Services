using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
namespace v380stream
{
    public class FlvStream
    {
        private readonly bool _enableAudio = true;
        private readonly bool _exit = false;
        private readonly Thread _thread;
        private static Semaphore _semaphore;
        enum EQueueType
        {
            Video,
            Audio,
            VideoKeyFrame
        }

        private struct QueueItem
        {
            public EQueueType Type;
            public List<byte> Packet;
        }
        public FlvStream()
        {
            _semaphore = new Semaphore(1, 1);
            _thread = new Thread(ProcessQueue);
            _thread.Start();
        }
        private void ProcessQueue()
        {
            var packetQueue = new Queue<QueueItem>();
            while (!_exit)
            {
                _semaphore.WaitOne();
                lock (packetQueue)
                {
                    while (packetQueue.Count > 0)
                    {
                        // Process packets
                    }
                }
                while (packetQueue.Count > 0)
                {
                    var queueItem = packetQueue.Dequeue();
                    ProcessQueueItem(queueItem);
                }
            }
        }
        private void ProcessQueueItem(QueueItem queueItem)
        {
            switch (queueItem.Type)
            {
                case EQueueType.Video:
                    WriteVideo(queueItem.Packet.ToArray(), false);
                    break;
                case EQueueType.VideoKeyFrame:
                    WriteVideo(queueItem.Packet.ToArray(), true);
                    break;
                case EQueueType.Audio:
                    WriteAudio(queueItem.Packet.ToArray());
                    break;
            }
        }

        private void WriteVideo(byte[] packet, bool isKeyFrame)
        {
            if (_exit || packet.Length == 0) return;

            using (var buffer = new MemoryStream())
            using (var writer = new BinaryWriter(buffer))
            {
                var dataSize = packet.Length;
                var timestamp = CalculateTimestamp(packet);

                WriteFlvTag(writer, dataSize, timestamp);

                var frameType = isKeyFrame ? (byte)0x10 : (byte)0x20;
                writer.Write((byte)(frameType | 0x07)); // Frame type and codec ID (assuming AVC)

                writer.Write(packet);

                WritePreviousTagSize(writer, dataSize);
                buffer.WriteTo(Console.OpenStandardOutput());
            }
        }

        private void WriteAudio(byte[] packet)
        {
            if (!_enableAudio || _exit || packet.Length == 0)
            {
                return;
            }
            using (MemoryStream buffer = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(buffer))
            {
                byte[] pcmData = DecodeAdpcm(packet);
                byte[] resampledData = ResamplePcm(pcmData, 8000, 11025);
                int timestamp = CalculateTimestamp(packet);
                WriteFlvTag(writer, resampledData.Length, timestamp);
                WriteFlvAudioData(writer, resampledData);
                WritePreviousTagSize(writer, resampledData.Length);
                buffer.WriteTo(Console.OpenStandardOutput());
            }
        }
        private byte[] DecodeAdpcm(byte[] adpcmData)
        {
            using MemoryStream ms = new MemoryStream(adpcmData);
            using WaveStream waveStream = new WaveFileReader(ms);
            using WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(waveStream);
            using MemoryStream pcmMs = new MemoryStream();
            pcmStream.CopyTo(pcmMs);
            return pcmMs.ToArray();
        }
        private byte[] ResamplePcm(byte[] pcmData, int inputRate, int outputRate)
        {
            using WaveFormatConversionStream resampler = new WaveFormatConversionStream(
                new WaveFormat(outputRate, 16, 1),
                new RawSourceWaveStream(new MemoryStream(pcmData), new WaveFormat(inputRate, 16, 1)));
            using MemoryStream resampledMs = new MemoryStream();
            resampler.CopyTo(resampledMs);
            return resampledMs.ToArray();
        }
        private int CalculateTimestamp(byte[] packet)
        {
            // Implement timestamp calculation logic here
            return 0;
        }
        private void WriteFlvTag(BinaryWriter writer, int dataSize, int timestamp)
        {
            writer.Write((byte)0x09); // Tag type (video)
            writer.Write((byte)((dataSize >> 16) & 0xFF)); // Data size (24-bit)
            writer.Write((byte)((dataSize >> 8) & 0xFF));
            writer.Write((byte)(dataSize & 0xFF));
            writer.Write((byte)((timestamp >> 16) & 0xFF)); // Timestamp (24-bit)
            writer.Write((byte)((timestamp >> 8) & 0xFF));
            writer.Write((byte)(timestamp & 0xFF));
            writer.Write((byte)((timestamp >> 24) & 0xFF)); // Timestamp extended
            writer.Write((byte)0x00); // Stream ID (always 0)
            writer.Write((byte)0x00);
            writer.Write((byte)0x00);
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
}
