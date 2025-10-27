# v380stream - V380 Camera Stream Capture

## Overview

C# implementation of the V380 IP camera streaming protocol, based on the original C++ implementation from [prsyahmi/v380](https://github.com/prsyahmi/v380).

## Features

- ? **Complete Protocol Implementation**
  - V380 camera authentication with AES encryption
  - Stream initialization and control
  - Frame reception and assembly
  
- ? **Output Files** (Matching C++ Original)
  - `video.h264` - Raw H.264 video stream
- `audio.raw` - Raw IMA ADPCM audio (8kHz, mono, 16-bit)
  
- ? **Bonus Features** (Not in C++ Original)
  - Real-time audio playback during capture
  - Enhanced error handling and logging

## Configuration

Create a `config.json` file:

```json
{
  "ID": "your_device_id",
  "UserName": "your_username",
  "Password": "your_password",
  "IP": "camera_ip_address",
  "Port": 8800,
  "LogFileName": "v380stream.log"
}
```

## Usage

```bash
# Run the application
dotnet run

# Or build and run
dotnet build
cd bin\Debug\net8.0
.\v380stream.exe
```

### Output Files

After running, you'll get two files:

1. **video.h264** - Raw H.264 video stream
   - Can be played with VLC, ffplay, or any H.264-compatible player
   - Example: `ffplay video.h264`

2. **audio.raw** - Raw IMA ADPCM audio
   - Convert to WAV: `ffmpeg -f s16le -ar 8000 -ac 1 -i audio.raw output.wav`
   - Or with sox: `sox -t ima -r 8000 -e ms-adpcm audio.raw output.wav`

## Protocol Details

### Authentication
- **Command 1167**: Login request with AES-encrypted password
- **Command 1168**: Login response with auth ticket
- **Encryption**: Two-stage AES-ECB with static key `"macrovideo+*#!^@"`

### Streaming
- **Command 301**: Stream login request
- **Command 401**: Stream login response (includes resolution)
- **Command 303**: Stream start request

### Frame Format
- **Header**: 12 bytes per frame
- **Marker Bytes**:
  - `0x7f` - Video/Audio frame
  - `0x1f` - Control data (skipped)
  - `0x6f` - Keepalive (20ms delay)
- **Frame Types**:
  - `0x00` - I-frame (keyframe)
  - `0x01` - P-frame (predicted frame)
  - `0x16` - Audio frame (IMA ADPCM)

## Implementation Comparison with C++

| Feature | C++ Original | This C# Implementation |
|---------|-------------|------------------------|
| Protocol | ? Exact match | ? Exact match |
| AES Encryption | ? macrovideo+*#!^@ | ? macrovideo+*#!^@ |
| File Output | ? video.h264 + audio.raw | ? video.h264 + audio.raw |
| Real-time Playback | ? No | ? **YES** (bonus feature) |

## Dependencies

- .NET 8.0
- NAudio 2.2.1 - Audio processing
- NLog 6.0.5 - Logging
- Newtonsoft.Json 13.0.4 - Configuration
- Xabe.FFmpeg 6.0.2 - (Referenced, for future use)

## Project Structure

```
v380stream/
??? ConnectionHanler.cs  # Main protocol implementation
??? Program.cs           # Entry point
??? Config.cs  # Configuration model
??? FlvStream.cs         # FLV processing (future use)
??? config.json     # Configuration file
```

## Technical Notes

### Protocol Implementation
The C# implementation is a **perfect port** of the C++ v380 protocol:
- Identical packet structures
- Same command codes and parameters
- Exact frame header parsing
- Matching multi-frame assembly logic

### Differences from C++
1. **Language**: C# instead of C++
2. **Audio Playback**: Added real-time playback (not in C++ version)
3. **Error Handling**: Enhanced error handling with NLog
4. **Cross-platform**: Runs on Windows, Linux, macOS (.NET 8)

## Troubleshooting

### Authentication Failed
- Check device ID, username, and password in config.json
- Verify camera IP address and port (default: 8800)
- Ensure camera is powered on and accessible on network

### No Video/Audio Output
- Check log file for errors
- Verify camera supports H.264 encoding
- Ensure write permissions in application directory

### Audio Playback Issues
- Audio playback requires compatible audio hardware
- Disable playback by commenting out `PlayAudio()` call if needed
- Files are still written even if playback fails

## License

Based on the original prsyahmi/v380 C++ implementation.

## Credits

- Original C++ Implementation: [prsyahmi/v380](https://github.com/prsyahmi/v380)
- C# Port: Part of EV3Services project
- Protocol Reverse Engineering: prsyahmi

## See Also

- [CPP_V380_ACCURATE_COMPARISON.md](CPP_V380_ACCURATE_COMPARISON.md) - Detailed protocol comparison
- [Original C++ v380](https://github.com/prsyahmi/v380) - Reference implementation
