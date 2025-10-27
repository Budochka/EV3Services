using RabbitMQ.Client.Events;
using MySqlConnector;
using System.Data;
using System.Threading.Tasks;

namespace Logger
{
    class Worker
 { 
    private readonly NLog.Logger _logs;
  private RabbitConsumer? _consumer;
   private readonly Config _config;
     private MySqlConnection? _connection;

        private bool _started;

public Worker(NLog.Logger log, Config config)
        {
        _logs = log;
      _config = config;
    _started = false;
    }

 public async Task InitializeAsync()
      {
            _consumer = new RabbitConsumer(_logs);
   await _consumer.ConnectToRabbitAsync(_config.RabbitUserName,
         _config.RabbitPassword,
     _config.RabbitHost,
  _config.RabbitPort,
   HandleRabbitMessageAsync);
    _logs.Info("RabbitConsumer created");

       _connection = new MySqlConnection(_config.ConnectionString);
      await _connection.OpenAsync();
_logs.Info("Database connection opened");
      }

  public void Start()
      {
       _started = true;
}

public void Stop()
  {
     _started = false;
    }

    private async Task HandleRabbitMessageAsync(object? sender, BasicDeliverEventArgs args)
{
  var bytes = args.Body.ToArray();
if ((bytes.Length > 0) && _started && (_connection != null))
   {
       using var command = _connection.CreateCommand();
      command.CommandText = @"insert into Events (Time, Topic, Data) values (@time, @topic, @data);";
          command.Parameters.Add(new MySqlParameter
     {
  ParameterName = "@time",
  DbType = DbType.DateTime,
    Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff")
            });
            command.Parameters.Add(new MySqlParameter
{
   ParameterName = "@topic",
  DbType = DbType.String,
    Value = args.RoutingKey
     });
  command.Parameters.Add(new MySqlParameter
   {
     ParameterName = "@data",
   DbType = DbType.Binary,
  Value = bytes
           });
    await command.ExecuteNonQueryAsync();
  }
        }
    }
}
