using System;
using System.IO;
using NLog;
using System.Text.Json;

namespace VoiceCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            Config config = new Config();

            //Load config file
            try
            {
                string jsonConfig = File.ReadAllText("config.json");
                config = JsonSerializer.Deserialize<Config>(jsonConfig);
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
                FileName = config.LogFileName,
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
            worker.Initialize();
            worker.Start();
            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
            worker.Stop();
        } // Main
    }
} // ns