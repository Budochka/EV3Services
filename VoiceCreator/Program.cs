using System;
using System.IO;
using NLog;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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
            var NlogConfig = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File and Console
            var logfile = new NLog.Targets.FileTarget("logfile")
            {
                FileName = config.LogFileName,
                DeleteOldFileOnStartup = true
            };

            // set logging rules
            NlogConfig.AddRuleForAllLevels(logfile);

            // Apply config           
            NLog.LogManager.Configuration = NlogConfig;

            new RabbitConnector(LogManager.GetCurrentClassLogger()).
                ConnectToRabbit(config.RabbitUserName, config.RabbitPassword, config.RabbitVHost, config.RabbitHost, config.RabbitPort, HandleRabbitMessage);

            VoiceSynthesis vc = new VoiceSynthesis(LogManager.GetCurrentClassLogger());

            while (true)
            {
                string s = Console.ReadLine();
                if (s == "done")
                {
                    break;
                }

                string file;

                vc.Text2File(s, out file);
                Console.WriteLine(file);

                var filedata = File.ReadAllBytes(file);
            }
        } // Main

        public static void HandleRabbitMessage(object sender, BasicDeliverEventArgs args)
        {
            string body = args.Body.ToString();
            if (body.Length > 0)
            {
                VoiceSynthesis vc = new VoiceSynthesis(LogManager.GetCurrentClassLogger());
                string file;

                vc.Text2File(body, out file);
                var filedata = File.ReadAllBytes(file);
            }

            ((IModel)sender).BasicAck(args.DeliveryTag, false);
        }
    }
} // ns