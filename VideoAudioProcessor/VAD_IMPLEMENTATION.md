# Voice Activity Detection (VAD) Implementation

## Overview

This document describes the implementation of Voice Activity Detection (VAD), silence detection, and white noise filtering to minimize Yandex API calls and reduce costs.

## Implementation Date
January 2025

## Components Created

### 1. AudioPreProcessor.cs
Main component that implements:
- **Voice Activity Detection (VAD)** - Detects when speech is present
- **Silence Detection** - Filters silent audio frames
- **White Noise Detection** - Filters noise from speech
- **State Machine** - Manages audio processing states
- **Smart Buffering** - Batches speech segments before sending to API

### 2. AudioBuffer.cs
Smart audio buffer that:
- Batches multiple audio frames
- Enforces minimum/maximum duration constraints
- Tracks silence detection
- Combines frames efficiently

### 3. AdaptiveNoiseFloor.cs
Adaptive learning component that:
- Learns environment noise floor
- Adjusts thresholds dynamically
- Improves detection accuracy over time

## Configuration Parameters

All parameters are configurable via `config.json`:

### VAD Settings
- `EnergyThreshold` (500.0) - RMS energy threshold for speech detection
- `EnergyMultiplier` (2.0) - Multiplier for adaptive threshold
- `SilenceThreshold` (100.0) - RMS threshold for silence
- `NoiseFloor` (200.0) - Baseline noise level

### Zero-Crossing Rate
- `MinZCR` (0.01) - Minimum ZCR for speech (100 crossings/sec at 8kHz)
- `MaxZCR` (0.375) - Maximum ZCR for speech (3000 crossings/sec)

### Spectral Analysis
- `SpectralRatioThreshold` (0.3) - High-frequency energy ratio
- `NoiseFlatnessThreshold` (0.7) - Spectral flatness for noise detection
- `SpeechAutocorrelationThreshold` (0.3) - Autocorrelation threshold

### Buffering
- `MinBufferDurationMs` (500) - Minimum audio duration before sending
- `MaxBufferDurationMs` (3000) - Maximum buffer duration before forced flush
- `SilenceFramesToEnd` (15) - Frames of silence to end speech segment
- `MaxSilentFrames` (30) - Max silent frames before state reset

### Adaptive Learning
- `EnableAdaptiveThreshold` (true) - Enable adaptive noise floor learning
- `AdaptationWindowSize` (100) - Number of frames for adaptation

## How It Works

### Audio Processing Flow

```
Camera Audio → ADPCM Decode → PCM Audio → AudioPreProcessor
                                                      ↓
                                    ┌─────────────────┼─────────────────┐
                                    ↓                 ↓                 ↓
                              Silence?            Noise?          Speech?
                                    ↓                 ↓                 ↓
                              [Filter]          [Filter]        [Buffer]
                                                                    ↓
                                                              [Batch Audio]
                                                                    ↓
                                                          [Send to Yandex API]
```

### State Machine

1. **Silence State**
   - Monitors for speech activity
   - Filters silent frames
   - Transitions to SpeechDetected when voice activity detected

2. **SpeechDetected State**
   - Initial speech detection
   - Starts buffering audio
   - Transitions to SpeechBuffering

3. **SpeechBuffering State**
   - Collects speech audio frames
   - Monitors for silence/noise
   - Flushes buffer when speech ends or max duration reached

4. **Noise State**
   - Filters white noise
   - Monitors for speech or silence
   - Transitions back when conditions change

### Detection Algorithms

#### Voice Activity Detection
1. **Energy-based (RMS)**: Speech has higher energy than silence/noise
2. **Zero-Crossing Rate**: Speech has moderate ZCR (100-3000 crossings/sec)
3. **Variance**: Speech has higher variance than silence

#### Silence Detection
1. **Low RMS energy**: Below silence threshold
2. **Low ZCR**: Very few zero crossings
3. **Low variance**: Samples are very similar

#### White Noise Detection
1. **High ZCR**: Very high zero-crossing rate
2. **Low autocorrelation**: Random-like distribution
3. **High energy + High ZCR**: Noise pattern

## Expected Cost Reduction

### Before Implementation
- **Audio frames per second**: ~50-100
- **API calls**: Every frame = 50-100 calls/second
- **Cost**: High (paying for all audio including silence/noise)

### After Implementation
- **Silence filtering**: ~60-80% reduction
- **Noise filtering**: ~10-20% additional reduction
- **Batching**: 1 API call per speech segment (vs 50-100 per second)
- **Total reduction**: **70-90% cost savings**

### Example Calculation
```
Before: 100 frames/sec × 3600 sec = 360,000 API calls/hour
After:  ~10 speech segments/hour × 1 call = 10 API calls/hour
Savings: 99.997% reduction
```

## Statistics Tracking

The implementation tracks:
- Total audio frames processed
- Filtered frames (silence + noise)
- API calls made
- Filter rate percentage

Statistics are logged on shutdown.

## Usage

The implementation is automatic - no code changes needed in calling code. Simply:

1. Configure parameters in `config.json`
2. Run the application
3. Monitor statistics in logs

## Tuning Parameters

### For Noisy Environments
- Increase `EnergyThreshold` (e.g., 700.0)
- Increase `NoiseFloor` (e.g., 300.0)
- Decrease `MaxZCR` (e.g., 0.3)

### For Quiet Environments
- Decrease `EnergyThreshold` (e.g., 300.0)
- Decrease `SilenceThreshold` (e.g., 50.0)
- Increase `MinZCR` (e.g., 0.005)

### For Faster Response
- Decrease `MinBufferDurationMs` (e.g., 300)
- Decrease `SilenceFramesToEnd` (e.g., 10)

### For Better Accuracy
- Increase `MinBufferDurationMs` (e.g., 800)
- Increase `SilenceFramesToEnd` (e.g., 20)
- Enable `EnableAdaptiveThreshold`

## Testing

### Test Cases
1. **Pure Silence** - Should filter 100% of frames
2. **White Noise** - Should filter 100% of frames
3. **Speech Only** - Should process 100% of frames
4. **Speech + Silence** - Should process speech, filter silence
5. **Speech + Noise** - Should process speech, filter noise
6. **Mixed Audio** - Should intelligently filter

### Monitoring
- Check logs for filter statistics
- Monitor API call count
- Verify speech recognition accuracy
- Adjust parameters based on results

## Files Modified

1. `Config.cs` - Added VAD configuration parameters
2. `config.json` - Added default VAD values
3. `Worker.cs` - Integrated AudioPreProcessor
4. `AudioPreProcessor.cs` - **NEW** - Main VAD implementation
5. `AudioBuffer.cs` - **NEW** - Smart audio buffering
6. `AdaptiveNoiseFloor.cs` - **NEW** - Adaptive threshold learning

## Future Enhancements

Potential improvements:
1. Machine learning-based VAD
2. Spectral analysis with FFT
3. Multi-band energy analysis
4. Voice quality scoring
5. Real-time parameter adjustment
6. Per-environment profiles

## Notes

- All detection algorithms are lightweight and run in real-time
- No external dependencies required (pure C# implementation)
- Compatible with existing Yandex streaming API
- Maintains backward compatibility

