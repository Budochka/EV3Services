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
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            logger.Info("Ctrl+C received, stopping stream...");
            connection.StopStreaming();
            Console.WriteLine("\nStream stopped. Files saved:");
            Console.WriteLine("  - video.h264 (raw H.264 stream)");
            Console.WriteLine("  - audio.raw (raw IMA ADPCM audio)");
            Console.WriteLine("\nTo play audio: ffmpeg -f s16le -ar 8000 -ac 1 -i audio.raw output.wav");
        };

        connection.StartStreaming(token);

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    public static void EnumerateCodecs()
    {
        foreach (var driver in AcmDriver.EnumerateAcmDrivers())
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Long Name: {0}\r\n", driver.LongName);
            builder.AppendFormat("Short Name: {0}\r\n", driver.ShortName);
            builder.AppendFormat("Driver ID: {0}\r\n", driver.DriverId);
            driver.Open();
            builder.AppendFormat("FormatTags:\r\n");
            foreach (AcmFormatTag formatTag in driver.FormatTags)
            {
                builder.AppendFormat("===========================================\r\n");
                builder.AppendFormat("Format Tag {0}: {1}\r\n", formatTag.FormatTagIndex, formatTag.FormatDescription);
                builder.AppendFormat("   Standard Format Count: {0}\r\n", formatTag.StandardFormatsCount);
                builder.AppendFormat("   Support Flags: {0}\r\n", formatTag.SupportFlags);
                builder.AppendFormat("   Format Tag: {0}, Format Size: {1}\r\n", formatTag.FormatTag, formatTag.FormatSize);
                builder.AppendFormat("   Formats:\r\n");
                foreach (AcmFormat format in driver.GetFormats(formatTag))
                {
                    builder.AppendFormat("   ===========================================\r\n");
                    builder.AppendFormat("   Format {0}: {1}\r\n", format.FormatIndex, format.FormatDescription);
                    builder.AppendFormat("      FormatTag: {0}, Support Flags: {1}\r\n", format.FormatTag, format.SupportFlags);
                    builder.AppendFormat("      WaveFormat: {0} {1}Hz Channels: {2} Bits: {3} Block Align: {4}, AverageBytesPerSecond: {5} ({6:0.0} kbps), Extra Size: {7}\r\n",
                        format.WaveFormat.Encoding, format.WaveFormat.SampleRate, format.WaveFormat.Channels,
                        format.WaveFormat.BitsPerSample, format.WaveFormat.BlockAlign, format.WaveFormat.AverageBytesPerSecond,
                        (format.WaveFormat.AverageBytesPerSecond * 8) / 1000.0,
                        format.WaveFormat.ExtraSize);
                    if (format.WaveFormat is WaveFormatExtraData && format.WaveFormat.ExtraSize > 0)
                    {
                        WaveFormatExtraData wfed = (WaveFormatExtraData)format.WaveFormat;
                        builder.Append("      Extra Bytes:\r\n      ");
                        for (int n = 0; n < format.WaveFormat.ExtraSize; n++)
                        {
                            builder.AppendFormat("{0:X2} ", wfed.ExtraData[n]);
                        }
                        builder.Append("\r\n");
                    }
                }
            }
            driver.Close();
            Console.WriteLine(builder.ToString());
        }
    }
}
