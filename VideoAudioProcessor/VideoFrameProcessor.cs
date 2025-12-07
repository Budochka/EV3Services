using System.Diagnostics;
using NLog;

namespace VideoAudioProcessor;

public class VideoFrameProcessor
{
    private readonly Logger _logs;
    private readonly int _targetWidth;
    private readonly int _jpegQuality;

    public VideoFrameProcessor(Logger log, int targetWidth, int jpegQuality)
    {
        _logs = log;
        _targetWidth = targetWidth;
        _jpegQuality = jpegQuality;
    }

    public async Task<byte[]?> ProcessH264FrameAsync(byte[] h264Data)
    {
        if (h264Data.Length == 0) return null;

        var tempH264 = Path.GetTempFileName() + ".h264";
        var tempJpeg = Path.GetTempFileName() + ".jpg";

        try
        {
            await File.WriteAllBytesAsync(tempH264, h264Data);

            // Use FFmpeg command-line to extract first frame and convert to JPEG with quality
            // FFmpeg -q:v uses scale 2-31 (2=best quality, 31=worst quality)
            // Map our 0-100 quality (100=best) to FFmpeg's 2-31 scale
            // Formula: scale = 2 + (31-2) * (100-quality) / 100
            int ffmpegQuality = 2 + (int)Math.Round(29.0 * (100 - _jpegQuality) / 100.0);
            ffmpegQuality = Math.Clamp(ffmpegQuality, 2, 31);

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-i \"{tempH264}\" -vf \"scale={_targetWidth}:-1\" -frames:v 1 -q:v {ffmpegQuality} -y \"{tempJpeg}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(processStartInfo);
            if (process == null)
            {
                _logs.Warn("Failed to start FFmpeg process");
                return null;
            }

            // Read stderr to avoid blocking (FFmpeg outputs to stderr)
            _ = process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                _logs.Warn($"FFmpeg exited with code {process.ExitCode}");
                return null;
            }

            if (!File.Exists(tempJpeg))
            {
                _logs.Warn("FFmpeg did not create output file");
                return null;
            }

            // Read the JPEG file directly (no re-encoding needed)
            var jpegData = await File.ReadAllBytesAsync(tempJpeg);
            return jpegData;
        }
        catch (FileNotFoundException ex)
        {
            _logs.Warn(ex, "FFmpeg executable not found. Ensure FFmpeg is installed and in PATH.");
            return null;
        }
        catch (InvalidOperationException ex)
        {
            _logs.Warn(ex, "Invalid H.264 frame data");
            return null;
        }
        catch (Exception ex)
        {
            _logs.Warn(ex, "Failed to process H.264 frame");
            return null;
        }
        finally
        {
            try
            {
                if (File.Exists(tempH264)) File.Delete(tempH264);
            }
            catch (Exception ex)
            {
                _logs.Trace(ex, "Failed to delete temp H264 file");
            }
            
            try
            {
                if (File.Exists(tempJpeg)) File.Delete(tempJpeg);
            }
            catch (Exception ex)
            {
                _logs.Trace(ex, "Failed to delete temp JPEG file");
            }
        }
    }
}

