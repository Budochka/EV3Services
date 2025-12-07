using NLog;
using RabbitMQ.Client;
using System;
using System.Threading.Tasks;

namespace EV3Services.Common;

/// <summary>
/// Shared RabbitMQ publisher class that eliminates code duplication across projects.
/// Supports flexible publishing with configurable exchange and routing keys.
/// </summary>
public class RabbitMQPublisher : IAsyncDisposable
{
    private readonly Logger _logs;
    private IChannel? _channel;
    private IConnection? _connection;
    private readonly string _exchange;

    public RabbitMQPublisher(Logger log, string exchange = "EV3")
    {
        _logs = log ?? throw new ArgumentNullException(nameof(log));
        _exchange = exchange;
    }

    /// <summary>
    /// Connects to RabbitMQ and declares the exchange.
    /// </summary>
    /// <param name="user">RabbitMQ username</param>
    /// <param name="pass">RabbitMQ password</param>
    /// <param name="hostName">RabbitMQ host</param>
    /// <param name="port">RabbitMQ port</param>
    /// <returns>True if connection successful, false otherwise</returns>
    public async Task<bool> ConnectToRabbitAsync(string user, string pass, string hostName, int port)
    {
        var factory = new ConnectionFactory()
        {
            UserName = user,
            Password = pass,
            HostName = hostName,
            Port = port,
            AutomaticRecoveryEnabled = true
        };

        _logs.Info("Creating Rabbit MQ connection host:{0}, port: {1}", hostName, port);
        
        try
        {
            _connection = await factory.CreateConnectionAsync();
            if (_connection == null)
            {
                _logs.Error("Connection is null after creation");
                return false;
            }

            _logs.Info("Connection created");
            _channel = await _connection.CreateChannelAsync();
            if (_channel == null)
            {
                _logs.Error("Channel is null after creation");
                return false;
            }

            _logs.Info("Channel created");

            // Declare exchange
            await _channel.ExchangeDeclareAsync(exchange: _exchange, type: "topic", autoDelete: true);
            _logs.Info("Exchange '{0}' created", _exchange);

            return true;
        }
        catch (Exception ex)
        {
            _logs.Error(ex, "Error creating RabbitMQ connection");
            return false;
        }
    }

    /// <summary>
    /// Publishes a message to the specified routing key.
    /// </summary>
    /// <param name="routingKey">Routing key for the message</param>
    /// <param name="data">Message body</param>
    public async Task PublishAsync(string routingKey, byte[] data)
    {
        if (_channel == null)
        {
            _logs.Warn("Cannot publish: channel is null");
            return;
        }

        try
        {
            await _channel.BasicPublishAsync(exchange: _exchange, routingKey: routingKey, body: data);
        }
        catch (Exception ex)
        {
            _logs.Warn(ex, "Failed to publish message with routing key '{0}'", routingKey);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _logs.Info("DisposeAsync called");

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

