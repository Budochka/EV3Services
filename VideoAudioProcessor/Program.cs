using NLog;
using System.Text.Json;
using System.Threading.Tasks;

namespace VideoAudioProcessor;

class Program
{
    static async Task Main(string[] args)
    {
        Config? config = null;

        // Load config file
        try
        {
            string jsonConfig = File.ReadAllText("config.json");
            config = JsonSerializer.Deserialize<Config>(jsonConfig);
            
            if (config == null)
            {
                Console.Error.WriteLine("Error: config.json is invalid or empty");
                Environment.Exit(-1);
            }

            // Load Yandex credentials from separate file
            try
            {
                string yandexConfig = File.ReadAllText("yandex-credentials.json");
                var yandexCreds = JsonSerializer.Deserialize<Dictionary<string, string>>(yandexConfig);
                if (yandexCreds?.TryGetValue("ApiKey", out var apiKey) == true)
                {
                    config = config with { YandexApiKey = apiKey };
                }
            }
            catch (FileNotFoundException)
            {
                Console.Error.WriteLine("Warning: yandex-credentials.json not found");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Warning: Could not load Yandex credentials: {0}", ex.Message);
            }
        }
        catch (FileNotFoundException)
        {
            Console.Error.WriteLine("Error: config.json not found");
            Environment.Exit(-1);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Error while reading config file: {0}", ex.Message);
            Environment.Exit(-1);
        }

        // Initialize logger
        var nlogConfig = new NLog.Config.LoggingConfiguration();

        var logfile = new NLog.Targets.FileTarget("logfile")
        {
            FileName = config?.LogFileName ?? "logfile.txt",
            DeleteOldFileOnStartup = true,
            Layout = "${longdate} : ${callsite} : ${message}"
        };

        var console = new NLog.Targets.ConsoleTarget()
        {
            DetectConsoleAvailable = true,
            Layout = "${longdate} : ${callsite} : ${message}"
        };

        nlogConfig.AddRuleForAllLevels(logfile);
        nlogConfig.AddRuleForAllLevels(console);

        NLog.LogManager.Configuration = nlogConfig;

        Worker worker = new Worker(LogManager.GetCurrentClassLogger(), config);
        await worker.InitializeAsync();
        worker.Start();
        Console.WriteLine("Press Enter to exit");
        Console.ReadLine();
        await worker.StopAsync();
    }
}

