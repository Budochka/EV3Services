using NLog;
using RabbitMQ.Client;
using System.Threading.Tasks;

namespace VideoAudioProcessor;

public class RabbitMQPublisher : IAsyncDisposable
{
    private readonly Logger _logs;
    private readonly Config _config;
    private IChannel? _channel;
    private IConnection? _connection;
    private volatile bool _connected = false;

    public RabbitMQPublisher(Logger log, Config config)
    {
        _logs = log;
        _config = config;
    }

    public async Task<bool> ConnectAsync()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                UserName = _config.RabbitUserName ?? "guest",
                Password = _config.RabbitPassword ?? "guest",
                HostName = _config.RabbitHost ?? "localhost",
                Port = _config.RabbitPort,
                AutomaticRecoveryEnabled = true
            };

            _connection = await factory.CreateConnectionAsync();
            if (_connection == null)
            {
                _logs.Error("Failed to create RabbitMQ connection");
                return false;
            }

            _channel = await _connection.CreateChannelAsync();
            if (_channel == null)
            {
                _logs.Error("Failed to create RabbitMQ channel");
                return false;
            }

            await _channel.ExchangeDeclareAsync(exchange: "EV3", type: "topic", autoDelete: true);
            _connected = true;
            _logs.Info("RabbitMQ connected");
            return true;
        }
        catch (Exception ex)
        {
            _logs.Error(ex, "Error connecting to RabbitMQ");
            return false;
        }
    }

    public async Task PublishVideoFrameAsync(byte[] jpegData)
    {
        if (!_connected || _channel == null) return;

        try
        {
            // Check queue depth before publishing (simple overflow protection)
            // Note: This is a basic check - for production, use RabbitMQ management API
            await _channel.BasicPublishAsync(
                exchange: "EV3",
                routingKey: "images.general",
                body: jpegData);
        }
        catch (Exception ex)
        {
            _logs.Warn(ex, "Failed to publish video frame, stopping writes");
            _connected = false; // Stop writing on error (queue overflow protection)
        }
    }

    public async Task PublishTextAsync(string text)
    {
        if (!_connected || _channel == null) return;

        try
        {
            var textBytes = System.Text.Encoding.UTF8.GetBytes(text);
            await _channel.BasicPublishAsync(
                exchange: "EV3",
                routingKey: "text.speech",
                body: textBytes);
        }
        catch (Exception ex)
        {
            _logs.Warn(ex, "Failed to publish text");
        }
    }

    public void Disconnect()
    {
        _connected = false;
    }

    public async ValueTask DisposeAsync()
    {
        _connected = false;
        if (_channel != null)
        {
            await _channel.CloseAsync();
            await _channel.DisposeAsync();
        }
        if (_connection != null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }
    }
}

