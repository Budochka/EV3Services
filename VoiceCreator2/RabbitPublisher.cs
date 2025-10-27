using NLog;
using RabbitMQ.Client;
using System.Threading.Tasks;

namespace VoiceCreator
{
  class RabbitPublisher : IAsyncDisposable
{
 private IChannel? _channel;
        private IConnection? _connection;
  private Logger _logs;

public RabbitPublisher(Logger log)
        {
     _logs = log;
      }

public async Task<bool> ConnectToRabbitAsync(string user, string pass, string hostName, int port)
{
 ConnectionFactory factory = new ConnectionFactory()
    {
   UserName = user,
   Password = pass,
     HostName = hostName,
 Port = port,
        AutomaticRecoveryEnabled = true
   };

            //creating connection
 _logs.Info("Creating Rabbit MQ connection host:{0}, port: {1}", hostName, port);
   _connection = await factory.CreateConnectionAsync();
  if (_connection != null)
       {
 _logs.Info("Connection created");
   _channel = await _connection.CreateChannelAsync();
 if (_channel != null)
  {
_logs.Info("Channel created");
        }
       else
     {
return false;
 }
}
else
{
return false;
  }

//declaring exchange 
   await _channel.ExchangeDeclareAsync(exchange: "EV3", type: "topic", autoDelete: true);
    _logs.Info("Exchange created");

return true;
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

public async Task PublishAsync(byte[] data)
    {
 if (_channel != null)
{
await _channel.BasicPublishAsync(exchange: "EV3", routingKey: "voice.wav", body: data);
}
}
    }
}