using Grpc.Net.Client;
using Grpc.Core;
using Yandex.Cloud.Ai.Tts.V3;

namespace VoiceCreator;

public class YandexSpeechKit
{
    private readonly string _apiKey;
    private readonly string _voice;
    private readonly string _language;
    private readonly string _emotion;
    private readonly double _speed;
    
    private byte[] _lastResponse = Array.Empty<byte>();
    private bool _success = false;

    public YandexSpeechKit(string apiKey, string voice = "filipp", 
        string language = "ru-RU", string emotion = "neutral", double speed = 1.0)
    {
        _apiKey = apiKey;
        _voice = voice;
        _language = language;
        _emotion = emotion;
        _speed = speed;
    }

    public byte[] LastResponse => _lastResponse;
    public bool IsSuccess => _success;

    public async Task SynthesizeAsync(string text)
    {
        _success = false;
        _lastResponse = Array.Empty<byte>();

        try
        {
            // Yandex SpeechKit gRPC endpoint
            const string endpoint = "https://tts.api.cloud.yandex.net:443";

            // Create gRPC channel with API key authentication
            var callCredentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                metadata.Add("authorization", $"Api-Key {_apiKey}");
                return Task.CompletedTask;
            });

            using var channel = GrpcChannel.ForAddress(endpoint, new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Create(
                    new SslCredentials(),
                    callCredentials)
            });

            var client = new Synthesizer.SynthesizerClient(channel);

            // Build request with WAV format
            // Note: Hints is a oneof, so we need separate Hints for each property
            // Language is typically determined by the voice name (e.g., "filipp" is Russian)
            // Emotion/role can be set via the "role" hint if needed
            var request = new UtteranceSynthesisRequest
            {
                Text = text,
                OutputAudioSpec = new AudioFormatOptions
                {
                    ContainerAudio = new ContainerAudio
                    {
                        ContainerAudioType = ContainerAudio.Types.ContainerAudioType.Wav
                    }
                },
                Hints =
                {
                    new Hints { Voice = _voice },
                    new Hints { Speed = _speed }
                    // Note: language is determined by voice, emotion can be set via role if supported
                },
                LoudnessNormalizationType = UtteranceSynthesisRequest.Types.LoudnessNormalizationType.Lufs
            };

            // Call gRPC method and collect audio chunks
            using var call = client.UtteranceSynthesis(request);
            using var audioStream = new MemoryStream();

            while (await call.ResponseStream.MoveNext())
            {
                var response = call.ResponseStream.Current;
                if (response.AudioChunk != null && response.AudioChunk.Data != null && response.AudioChunk.Data.Length > 0)
                {
                    audioStream.Write(response.AudioChunk.Data.ToByteArray());
                }
            }

            _lastResponse = audioStream.ToArray();
            _success = _lastResponse.Length > 0;
        }
        catch
        {
            // On error, remain silent (no action as per requirements)
        }
    }
}
