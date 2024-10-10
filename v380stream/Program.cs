﻿using NLog;
using System.Text.Json;

namespace v380stream;

class Program
{
    static void Main()
    {
        Config config = new();

        //Load config file
        try
        {
            string jsonConfig = File.ReadAllText("config.json");
            config = JsonSerializer.Deserialize<Config>(jsonConfig) ?? throw new InvalidOperationException();
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

        // set logging rules
        nlogConfig.AddRuleForAllLevels(logfile);

        // Apply config           
        NLog.LogManager.Configuration = nlogConfig;

        var connection = new ConnectionHanler(LogManager.GetCurrentClassLogger(), config);
        var res = connection.Authorise();

        Console.WriteLine(res);
        Console.ReadKey();
    }
}
