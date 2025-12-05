using Grpc.Net.Client;
using Grpc.Core;
using NLog;
using Yandex.Cloud.Ai.Stt.V3;

namespace VideoAudioProcessor;

public class YandexSpeechRecognizer
{
    private readonly Logger _logs;
    private readonly string _apiKey;
    private readonly string _language;
    private GrpcChannel? _channel;
    private Recognizer.RecognizerClient? _client;
    private AsyncDuplexStreamingCall<StreamingRequest, StreamingResponse>? _stream;
    private Task? _responseReaderTask;
    private volatile bool _isRecognizing = false;

    public event EventHandler<string>? TextRecognized;

    public YandexSpeechRecognizer(Logger log, string apiKey, string language = "ru-RU")
    {
        _logs = log;
        _apiKey = apiKey;
        _language = language;
    }

    public async Task StartRecognitionAsync()
    {
        try
        {
            const string endpoint = "https://stt.api.cloud.yandex.net:443";

            var callCredentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                metadata.Add("authorization", $"Api-Key {_apiKey}");
                return Task.CompletedTask;
            });

            _channel = GrpcChannel.ForAddress(endpoint, new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Create(
                    new SslCredentials(),
                    callCredentials)
            });

            _client = new Recognizer.RecognizerClient(_channel);

            _stream = _client.RecognizeStreaming();

            // Send session options
            var sessionOptions = new StreamingOptions
            {
                RecognitionModel = new RecognitionModelOptions
                {
                    Model = "general",
                    AudioFormat = new AudioFormatOptions
                    {
                        RawAudio = new RawAudio
                        {
                            AudioEncoding = RawAudio.Types.AudioEncoding.Linear16Pcm,
                            SampleRateHertz = 8000
                        }
                    },
                    LanguageRestriction = new LanguageRestrictionOptions
                    {
                        RestrictionType = LanguageRestrictionOptions.Types.LanguageRestrictionType.Whitelist,
                        LanguageCode = { _language }
                    },
                    AudioProcessingType = RecognitionModelOptions.Types.AudioProcessingType.RealTime
                },
                EouClassifier = new EouClassifierOptions
                {
                    DefaultClassifier = new DefaultEouClassifier
                    {
                        Type = DefaultEouClassifier.Types.EouSensitivity.Default
                    }
                }
            };

            await _stream.RequestStream.WriteAsync(new StreamingRequest
            {
                SessionOptions = sessionOptions
            });

            _isRecognizing = true;

            // Start reading responses
            _responseReaderTask = Task.Run(async () =>
            {
                try
                {
                    await foreach (var response in _stream.ResponseStream.ReadAllAsync())
                    {
                        if (response.Final?.Alternatives is { Count: > 0 })
                        {
                            var text = response.Final.Alternatives[0].Text;
                            if (!string.IsNullOrWhiteSpace(text))
                            {
                                _logs.Info($"Recognized text: {text}");
                                TextRecognized?.Invoke(this, text);
                            }
                        }
                    }
                }
                catch (RpcException ex)
                {
                    _logs.Error(ex, "gRPC error reading recognition responses");
                }
                catch (Exception ex)
                {
                    _logs.Error(ex, "Error reading recognition responses");
                }
            });

            _logs.Info("Speech recognition started");
        }
        catch (Exception ex)
        {
            _logs.Error(ex, "Failed to start speech recognition");
            _isRecognizing = false;
        }
    }

    public async Task AddAudioChunkAsync(byte[] audioData)
    {
        if (!_isRecognizing || _stream == null || audioData.Length == 0) return;

        try
        {
            await _stream.RequestStream.WriteAsync(new StreamingRequest
            {
                Chunk = new AudioChunk { Data = Google.Protobuf.ByteString.CopyFrom(audioData) }
            });
        }
        catch (RpcException ex)
        {
            _logs.Warn(ex, "gRPC error sending audio chunk");
            _isRecognizing = false;
        }
        catch (InvalidOperationException ex)
        {
            _logs.Warn(ex, "Stream closed, cannot send audio chunk");
            _isRecognizing = false;
        }
        catch (Exception ex)
        {
            _logs.Warn(ex, "Failed to send audio chunk");
        }
    }

    public async Task StopRecognitionAsync()
    {
        _isRecognizing = false;

        try
        {
            if (_stream != null)
            {
                await _stream.RequestStream.CompleteAsync();
            }

            if (_responseReaderTask != null)
            {
                await _responseReaderTask;
            }

            if (_channel != null)
            {
                await _channel.ShutdownAsync();
            }
        }
        catch (Exception ex)
        {
            _logs.Error(ex, "Error stopping recognition");
        }
    }
}

