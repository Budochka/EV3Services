using NLog;
using System.Text.Json;
using System.Threading.Tasks;

namespace VoiceCreator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Config? config = new Config();

            //Load config file
            try
            {
                string jsonConfig = File.ReadAllText("config.json");
                config = JsonSerializer.Deserialize<Config>(jsonConfig);
                
                // Load Yandex credentials from separate file
                try
                {
                    string yandexConfig = File.ReadAllText("yandex-credentials.json");
                    var yandexCreds = JsonSerializer.Deserialize<Dictionary<string, string>>(yandexConfig);
                    if (yandexCreds != null && yandexCreds.ContainsKey("ApiKey") && config != null)
                    {
                        config = new Config
                        {
                            LogFileName = config.LogFileName,
                            RabbitUserName = config.RabbitUserName,
                            RabbitPassword = config.RabbitPassword,
                            RabbitHost = config.RabbitHost,
                            RabbitPort = config.RabbitPort,
                            YandexApiKey = yandexCreds["ApiKey"]
                        };
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Warning: Could not load Yandex credentials: {0}", ex.Message);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error while reading config file {0}", ex.Message);
                System.Environment.Exit(-1);
            }

            //Initialize logger
            var nlogConfig = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File and Console
            var logfile = new NLog.Targets.FileTarget("logfile")
            {
                FileName = config?.LogFileName,
                DeleteOldFileOnStartup = true,
                Layout = "${longdate} : ${callsite} : ${message}"
            };

            var console = new NLog.Targets.ConsoleTarget()
            {
                DetectConsoleAvailable = true,
                Layout = "${longdate} : ${callsite} : ${message}"
            };

            // set logging rules
            nlogConfig.AddRuleForAllLevels(logfile);
            nlogConfig.AddRuleForAllLevels(console);

            // Apply config      
            NLog.LogManager.Configuration = nlogConfig;

            Worker worker = new Worker(LogManager.GetCurrentClassLogger(), config);
            await worker.InitializeAsync();
            worker.Start();
            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
            worker.Stop();
        } // Main
    }
} // ns