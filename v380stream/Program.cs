using NAudio.Wave.Compression;
using NAudio.Wave;
using NLog;
using System.Text;
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

        var logger = LogManager.GetCurrentClassLogger();
        var connection = new ConnectionHanler(logger, config);
        
        Console.WriteLine("V380 Stream Capture - Press Ctrl+C to stop");
        Console.WriteLine("Output files: video.h264, audio.raw");
        Console.WriteLine();
        
        var token = connection.Authorise();
        if (token == 0)
        {
            logger.Error("Authentication failed");
            Console.WriteLine("Authentication failed. Check credentials and try again.");
            return;
        }
    
        Console.WriteLine($"Authentication successful. Token: {token}");
        Console.WriteLine("Starting stream capture...");

        // Set up Ctrl+C handler for clean shutdown
        bool streamStopped = false;
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            if (!streamStopped)
            {
                streamStopped = true;
                logger.Info("Ctrl+C received, stopping stream...");
                connection.StopStreaming();
                Console.WriteLine("\nStream stopped. Files saved:");
                Console.WriteLine("  - video.h264 (raw H.264 stream)");
                Console.WriteLine("  - audio.raw (raw IMA ADPCM audio)");
                Console.WriteLine("\nTo convert audio:");
                Console.WriteLine("  PowerShell command (from any location):");
                Console.WriteLine("    .\\convert_audio.ps1 \"path\\to\\audio.raw\"");
                Console.WriteLine("  Or with C++ FLV approach (skip 18 bytes):");
                Console.WriteLine("    .\\convert_audio.ps1 \"path\\to\\audio.raw\" -SkipBytes 18");
                Console.WriteLine("  Or from script directory:");
                Console.WriteLine("    .\\convert_audio.ps1  (uses audio.raw in current dir, default skip 20)");
                Console.WriteLine("Exiting...");
                Environment.Exit(0);
            }
        };

        connection.StartStreaming(token);
        
        // If StartStreaming returns normally (stream ended), exit
        Console.WriteLine("Stream ended. Files saved.");
        Console.WriteLine("Exiting...");
    }
}
