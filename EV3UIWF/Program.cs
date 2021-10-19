using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using NLog;

namespace EV3UIWF
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

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

            // set logging rules
            nlogConfig.AddRuleForAllLevels(logfile);

            // Apply config           
            NLog.LogManager.Configuration = nlogConfig;
            
            Application.Run(new frmMain(LogManager.GetCurrentClassLogger(), config));
        }
    }
}
