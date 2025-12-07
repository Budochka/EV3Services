using System.Collections.Generic;

namespace VideoAudioProcessor;

/// <summary>
/// Smart audio buffer that batches audio frames before sending to API.
/// Implements minimum/maximum duration constraints and silence detection.
/// </summary>
public class AudioBuffer
{
    private readonly Queue<byte[]> _buffer = new();
    private readonly int _maxBufferDurationMs;
    private readonly int _minBufferDurationMs;
    private DateTime _firstFrameTime = DateTime.MinValue;
    private int _totalBytes = 0;
    private bool _silenceDetected = false;

    public AudioBuffer(int minBufferDurationMs, int maxBufferDurationMs)
    {
        _minBufferDurationMs = minBufferDurationMs;
        _maxBufferDurationMs = maxBufferDurationMs;
    }

    public void AddFrame(byte[] pcmData)
    {
        if (_firstFrameTime == DateTime.MinValue)
        {
            _firstFrameTime = DateTime.UtcNow;
        }

        _buffer.Enqueue(pcmData);
        _totalBytes += pcmData.Length;
        _silenceDetected = false; // Reset silence flag when adding frame
    }

    public void MarkSilence()
    {
        _silenceDetected = true;
    }

    public bool ShouldFlush()
    {
        if (_buffer.Count == 0) return false;

        var duration = DateTime.UtcNow - _firstFrameTime;

        // Flush if:
        // 1. Buffer is full (max duration reached)
        // 2. Silence detected after speech (end of utterance) AND minimum duration reached
        return duration.TotalMilliseconds >= _maxBufferDurationMs ||
               (_silenceDetected && duration.TotalMilliseconds >= _minBufferDurationMs);
    }

    public byte[] Flush()
    {
        if (_buffer.Count == 0)
        {
            return Array.Empty<byte>();
        }

        // Combine all buffered frames into single array
        var result = new byte[_totalBytes];
        int offset = 0;

        while (_buffer.Count > 0)
        {
            var frame = _buffer.Dequeue();
            Buffer.BlockCopy(frame, 0, result, offset, frame.Length);
            offset += frame.Length;
        }

        _totalBytes = 0;
        _firstFrameTime = DateTime.MinValue;
        _silenceDetected = false;

        return result;
    }

    public void Clear()
    {
        _buffer.Clear();
        _totalBytes = 0;
        _firstFrameTime = DateTime.MinValue;
        _silenceDetected = false;
    }

    public int FrameCount => _buffer.Count;
    public int TotalBytes => _totalBytes;
    public TimeSpan Duration => _firstFrameTime == DateTime.MinValue 
        ? TimeSpan.Zero 
        : DateTime.UtcNow - _firstFrameTime;
}

