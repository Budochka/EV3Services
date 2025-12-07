using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EV3Services.Common;

/// <summary>
/// Shared RabbitMQ consumer class that eliminates code duplication across projects.
/// Supports configurable queue bindings and auto-ack settings.
/// </summary>
public class RabbitMQConsumer : IAsyncDisposable
{
    private readonly Logger _logs;
    private IChannel? _channel;
    private IConnection? _connection;

    public RabbitMQConsumer(Logger log)
    {
        _logs = log ?? throw new ArgumentNullException(nameof(log));
    }

    /// <summary>
    /// Connects to RabbitMQ and sets up a consumer with the specified queue bindings.
    /// </summary>
    /// <param name="user">RabbitMQ username</param>
    /// <param name="pass">RabbitMQ password</param>
    /// <param name="hostName">RabbitMQ host</param>
    /// <param name="port">RabbitMQ port</param>
    /// <param name="eventHandler">Event handler for received messages</param>
    /// <param name="routingKeys">List of routing keys to bind to (e.g., ["sensors.*", "state.*"])</param>
    /// <param name="exchange">Exchange name (default: "EV3")</param>
    /// <param name="traceExchange">If true, binds to amq.rabbitmq.trace instead of regular exchange (for Logger)</param>
    /// <param name="autoAck">Whether to auto-acknowledge messages (default: false)</param>
    /// <returns>True if connection successful, false otherwise</returns>
    public async Task<bool> ConnectToRabbitAsync(
        string user,
        string pass,
        string hostName,
        int port,
        AsyncEventHandler<BasicDeliverEventArgs> eventHandler,
        IEnumerable<string>? routingKeys = null,
        string exchange = "EV3",
        bool traceExchange = false,
        bool autoAck = false)
    {
        if (eventHandler == null)
            throw new ArgumentNullException(nameof(eventHandler));

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

            // Declare queue
            var queueDeclareResult = await _channel.QueueDeclareAsync();
            var queueName = queueDeclareResult.QueueName;

            if (!traceExchange)
            {
                // Declare regular exchange
                await _channel.ExchangeDeclareAsync(exchange: exchange, type: "topic", autoDelete: true);
                _logs.Info("Exchange '{0}' created", exchange);

                // Bind routing keys
                if (routingKeys != null)
                {
                    foreach (var routingKey in routingKeys)
                    {
                        await _channel.QueueBindAsync(queue: queueName, exchange: exchange, routingKey: routingKey);
                        _logs.Debug("Queue bound to exchange '{0}' with routing key '{1}'", exchange, routingKey);
                    }
                }

                _logs.Info("Queue binding complete");
            }
            else
            {
                // For Logger: bind to RabbitMQ trace exchange (amq.rabbitmq.trace)
                // This exchange already exists (internal, durable), we just bind to it
                await _channel.QueueBindAsync(queue: queueName, exchange: "amq.rabbitmq.trace", routingKey: "#");
                _logs.Info("Queue bound to amq.rabbitmq.trace exchange (Firehose Tracer)");
            }

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += eventHandler;
            await _channel.BasicConsumeAsync(queue: queueName, autoAck: autoAck, consumer: consumer);

            _logs.Info("Event handler set");
            return true;
        }
        catch (Exception ex)
        {
            _logs.Error(ex, "Error creating RabbitMQ connection");
            return false;
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

