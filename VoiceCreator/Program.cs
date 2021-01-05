using System;
using System.IO;
using NLog;

namespace VoiceCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            //Initialize logger
            var config = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File and Console
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "file.txt" };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            // Rules for mapping loggers to targets            
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            // Apply config           
            NLog.LogManager.Configuration = config;

            //
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
    }
} // ns