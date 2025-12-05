# EV3Services

A distributed microservices architecture for controlling and monitoring LEGO EV3 robots with advanced computer vision, speech processing, and sensor integration capabilities.

## Overview

EV3Services is a message-driven robotics platform that orchestrates multiple services to enable autonomous robot behavior, real-time sensor processing, computer vision, and human-robot interaction. The system uses **RabbitMQ** as the central message bus, allowing services to communicate asynchronously and scale independently.

## Architecture

### System Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                    RabbitMQ Message Bus                        │
│                    (EV3 Topic Exchange)                        │
└──────────────┬──────────────────────────────────┬───────────────┘
               │                                  │
    ┌──────────▼──────────┐          ┌───────────▼──────────┐
    │   Input Services    │          │  Processing Services │
    └─────────────────────┘          └──────────────────────┘
    • EV3UIWF (UI)                   • Processor (State Machine)
    • VideoAudioProcessor            • FaceDetector
    • v380stream                     • FaceRecognizer
    • EV3Handler2 (Sensors)          • VoiceCreator2
                                     • VideoAudioProcessor
    ┌─────────────────────┐          └──────────────────────┘
    │  Output Services    │
    └─────────────────────┘
    • EV3Handler2 (Actuators)
    • Logger (Database)
```

### Design Principles

- **Message-Driven Architecture**: All services communicate via RabbitMQ
- **Microservices**: Each service has a single, well-defined responsibility
- **Language Agnostic**: Services written in C#, Python, and C++ as appropriate
- **Event-Driven**: Services react to events rather than polling
- **SOLID & KISS**: Simple, maintainable code without overengineering

## Services

### Core Services

#### 1. **Processor** (C# .NET 8.0)
- **Purpose**: Central state machine and message orchestrator
- **Responsibilities**:
  - Maintains robot world model and state
  - Routes messages to appropriate handlers
  - Coordinates robot behavior based on sensor inputs
- **Message Handlers**:
  - `TouchHandler`: Processes touch sensor events
  - `DistanceHandler`: Processes ultrasonic sensor data
  - `FacesHanler`: Processes face detection/recognition results
  - `StateHandler`: Manages robot state transitions
- **Routing Keys**: Consumes from multiple keys, publishes movement commands

#### 2. **EV3Handler2** (Python 3.11)
- **Purpose**: Direct interface to LEGO EV3 hardware
- **Responsibilities**:
  - Controls motors (movement, head rotation)
  - Reads sensors (touch, ultrasonic, color)
  - Plays audio (WAV files from RabbitMQ)
- **Features**:
  - Multi-threaded sensor reporting
  - Real-time motor control
  - Hardware abstraction via `ev3dev2`
- **Routing Keys**:
  - Consumes: `voice.wav`, `movement.*`
  - Publishes: `sensors.touch`, `sensors.distance`, `sensors.color`

#### 3. **Logger** (C# .NET 8.0)
- **Purpose**: Centralized event logging to database
- **Responsibilities**:
  - Consumes all RabbitMQ messages
  - Stores events in MySQL database
  - Provides audit trail for robot operations
- **Database**: MySQL with `Events` table (Time, Topic, Data)

### Computer Vision Services

#### 4. **VideoAudioProcessor** (C# .NET 8.0)
- **Purpose**: Unified video and audio processing with speech recognition
- **Features**:
  - Captures H.264 video from V380 cameras
  - Converts video frames to JPEG images
  - Decodes IMA ADPCM audio to PCM
  - Real-time speech recognition using Yandex SpeechKit
- **Routing Keys**:
  - Publishes: `images.general` (JPEG frames), `text.speech` (recognized text)
- **Dependencies**: FFmpeg, Yandex SpeechKit API

#### 5. **FaceDetector** (C++ with dlib)
- **Purpose**: Face detection in images
- **Features**:
  - Detects faces in JPEG images
  - Extracts face chips (150x150px)
  - Uses dlib face detection models
- **Routing Keys**:
  - Consumes: `images.general`
  - Publishes: `images.face` (detected faces)

#### 6. **FaceRecognizer** (C++ with dlib)
- **Purpose**: Face recognition and identification
- **Features**:
  - Recognizes known faces
  - Uses dlib face recognition models
  - Compares faces against known database
- **Routing Keys**:
  - Consumes: `images.face`
  - Publishes: Recognition results

#### 7. **v380stream** (C# .NET 8.0)
- **Purpose**: V380 IP camera stream capture utility
- **Features**:
  - Captures H.264 video and IMA ADPCM audio
  - Writes raw streams to files
  - Reference implementation for camera protocol
- **Output**: `video.h264`, `audio.wav`

### Speech Services

#### 8. **VoiceCreator2** (C# .NET 8.0)
- **Purpose**: Text-to-speech synthesis
- **Features**:
  - Converts text to WAV audio using Yandex SpeechKit
  - Supports Russian language (`filipp` voice)
  - Publishes audio to RabbitMQ
- **Routing Keys**:
  - Consumes: Text messages
  - Publishes: `voice.wav` (audio data)

### User Interface

#### 9. **EV3UIWF** (C# Windows Forms)
- **Purpose**: Windows desktop application for robot control
- **Features**:
  - Visual interface for robot control
  - Plugin system for extensibility
  - Real-time message monitoring
  - Command publishing to RabbitMQ
- **Plugins**: Python scripts in `Plugins/` folder

### Legacy Services

#### 10. **VideoConverterPY** (Python 3.11)
- **Status**: ⚠️ **Deprecated** - Replaced by `VideoAudioProcessor`
- **Purpose**: Legacy video frame conversion (OpenCV-based)
- **Note**: Kept for reference, not recommended for new deployments

## Message Bus Architecture

### RabbitMQ Exchange

- **Exchange Name**: `EV3`
- **Exchange Type**: `topic` (auto-delete)
- **Pattern**: Topic-based routing with wildcards

### Routing Keys

| Routing Key | Publisher | Consumer | Content |
|-------------|-----------|----------|---------|
| `images.general` | VideoAudioProcessor | FaceDetector, Logger | JPEG image bytes |
| `images.face` | FaceDetector | FaceRecognizer, Logger | JPEG face image bytes |
| `text.speech` | VideoAudioProcessor | Processor, Logger | UTF-8 text (recognized speech) |
| `voice.wav` | VoiceCreator2 | EV3Handler2, Logger | WAV audio bytes |
| `movement.turn` | Processor, EV3UIWF | EV3Handler2 | Movement command (turn) |
| `movement.distance` | Processor, EV3UIWF | EV3Handler2 | Movement command (forward/back) |
| `movement.headturn` | Processor, EV3UIWF | EV3Handler2 | Head rotation command |
| `sensors.touch` | EV3Handler2 | Processor, Logger | Touch sensor state |
| `sensors.distance` | EV3Handler2 | Processor, Logger | Ultrasonic distance (cm) |
| `sensors.color` | EV3Handler2 | Processor, Logger | Color sensor data |

## Technology Stack

### Languages & Frameworks

- **C# .NET 8.0**: Core services, state machine, UI
- **Python 3.11**: EV3 hardware interface, legacy services
- **C++**: High-performance computer vision (dlib)

### Key Libraries

- **RabbitMQ.Client**: Message bus communication
- **NLog**: Logging framework
- **dlib**: Face detection and recognition
- **ev3dev2**: LEGO EV3 Python library
- **Yandex SpeechKit**: Speech recognition and synthesis
- **FFmpeg**: Video/audio processing
- **MySQL**: Event logging database

### External Services

- **RabbitMQ**: Message broker (required)
- **MySQL**: Database for logging (required)
- **Yandex SpeechKit API**: Speech processing (optional)
- **FFmpeg**: Video processing (required for VideoAudioProcessor)

## Project Structure

```
EV3Services/
├── Processor/              # Main state machine service
├── EV3Handler2/            # EV3 hardware interface (Python)
├── Logger/                 # Event logging service
├── VideoAudioProcessor/    # Video/audio processing + speech recognition
├── VoiceCreator2/         # Text-to-speech service
├── FaceDetector/          # Face detection (C++)
├── FaceRecognizer/        # Face recognition (C++)
├── EV3UIWF/               # Windows Forms UI
├── v380stream/            # Camera stream capture utility
├── VideoConverterPY/      # Legacy video converter (deprecated)
├── Database/              # MySQL schema
├── Docs/                  # Architecture diagrams
├── Plugins/               # Python plugins for UI
└── Bin/                   # Build output directory
```

## Getting Started

### Prerequisites

1. **.NET 8.0 SDK**: For C# services
2. **Python 3.11**: For Python services
3. **Visual Studio 2022** or **Visual Studio Code**: For C++ services
4. **RabbitMQ Server**: Message broker
5. **MySQL Server**: For event logging
6. **FFmpeg**: For video processing (if using VideoAudioProcessor)
7. **LEGO EV3**: Robot hardware with ev3dev2 installed

### Installation

1. **Clone the repository**:
   ```bash
   git clone <repository-url>
   cd EV3Services
   ```

2. **Install RabbitMQ**:
   - Download from https://www.rabbitmq.com/download.html
   - Start RabbitMQ server

3. **Setup MySQL Database**:
   ```sql
   CREATE DATABASE ev3;
   USE ev3;
   SOURCE Database/ev3.sql;
   ```

4. **Configure Services**:
   - Each service has a `config.json` file
   - Update RabbitMQ and database credentials
   - For services using Yandex SpeechKit, create `yandex-credentials.json`

5. **Build Services**:
   ```bash
   # Build all C# services
   dotnet build EV3Services.sln
   
   # Build C++ services (requires Visual Studio)
   # Open FaceDetector/FaceDetector.vcxproj and build
   # Open FaceRecognizer/FaceRecognizer.vcxproj and build
   ```

6. **Install Python Dependencies**:
   ```bash
   cd EV3Handler2
   pip install -r requirements.txt
   ```

### Running the System

1. **Start RabbitMQ** (if not running as service)

2. **Start Core Services** (in order):
   ```bash
   # Logger (logs all events)
   cd Logger
   dotnet run
   
   # Processor (state machine)
   cd Processor
   dotnet run
   
   # EV3Handler2 (hardware interface)
   cd EV3Handler2
   python Main.py
   ```

3. **Start Vision Services** (optional):
   ```bash
   # VideoAudioProcessor
   cd VideoAudioProcessor
   dotnet run
   
   # FaceDetector
   cd FaceDetector
   ./FaceDetector.exe
   
   # FaceRecognizer
   cd FaceRecognizer
   ./FaceRecognizer.exe
   ```

4. **Start Speech Services** (optional):
   ```bash
   # VoiceCreator2
   cd VoiceCreator2
   dotnet run
   ```

5. **Start UI** (optional):
   ```bash
   cd EV3UIWF
   dotnet run
   ```

## Configuration

### Service Configuration Pattern

All services follow a consistent configuration pattern:

```json
{
  "LogFileName": "service.log",
  "RabbitUserName": "guest",
  "RabbitPassword": "guest",
  "RabbitHost": "localhost",
  "RabbitPort": 5672,
  // Service-specific settings...
}
```

### Yandex SpeechKit Configuration

Services using Yandex SpeechKit require a separate credentials file:

**`yandex-credentials.json`** (git-ignored):
```json
{
  "ApiKey": "your-yandex-api-key-here"
}
```

## Development

### Adding a New Service

1. **Create service project** following existing patterns:
   - C#: Use `Logger` or `Processor` as template
   - Python: Use `EV3Handler2` as template
   - C++: Use `FaceDetector` as template

2. **Implement Worker pattern**:
   - `InitializeAsync()`: Setup RabbitMQ connections
   - `Start()`: Begin processing
   - `Stop()`: Graceful shutdown

3. **Add to solution**:
   - C#: Add to `EV3Services.sln`
   - Python: Add `.pyproj` to solution
   - C++: Add `.vcxproj` to solution

4. **Configure routing keys**:
   - Document consumed and published keys
   - Update this README

### Testing

- **Unit Tests**: Located in `*.Tests` projects
- **Integration**: Test via RabbitMQ message flow
- **Hardware Testing**: Requires physical EV3 robot

## Architecture Patterns

### Message Flow Example: Voice Command

```
User Input (EV3UIWF)
    ↓
    Publishes: text message
    ↓
VoiceCreator2
    ↓
    Consumes: text
    Synthesizes: WAV audio
    Publishes: voice.wav
    ↓
EV3Handler2
    ↓
    Consumes: voice.wav
    Plays: Audio on robot
```

### Message Flow Example: Face Detection

```
VideoAudioProcessor
    ↓
    Captures: Camera frame
    Publishes: images.general
    ↓
FaceDetector
    ↓
    Consumes: images.general
    Detects: Faces
    Publishes: images.face
    ↓
FaceRecognizer
    ↓
    Consumes: images.face
    Recognizes: Known faces
    Publishes: Recognition result
    ↓
Processor
    ↓
    Consumes: Recognition result
    Updates: World model
    Decides: Robot action
```

## Troubleshooting

### RabbitMQ Connection Issues

- **Check RabbitMQ is running**: `rabbitmqctl status`
- **Verify credentials**: Check `config.json` in each service
- **Check firewall**: Port 5672 must be accessible

### EV3 Hardware Issues

- **Verify ev3dev2**: SSH to robot, check `/sys/class/`
- **Check USB/Bluetooth**: Connection to robot
- **Review EV3Handler2 logs**: `ev3handler.log`

### Video/Audio Processing Issues

- **FFmpeg not found**: Install FFmpeg and add to PATH
- **Camera connection**: Check V380 camera IP and credentials
- **Yandex API errors**: Verify API key in `yandex-credentials.json`

## Contributing

1. Follow existing code patterns
2. Maintain SOLID and KISS principles
3. Add tests for new features
4. Update documentation
5. Use consistent logging (NLog for C#, logging module for Python)

## License

[Specify your license here]

## Credits

- **LEGO EV3**: Hardware platform
- **ev3dev2**: Python library for EV3
- **dlib**: Face detection and recognition
- **Yandex SpeechKit**: Speech processing
- **RabbitMQ**: Message broker

## Related Projects

- **ev3dev**: Linux kernel for EV3
- **ev3dev2**: Python library for EV3 programming

