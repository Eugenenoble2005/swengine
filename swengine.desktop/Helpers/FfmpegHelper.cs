using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using swengine.desktop.Models;

namespace swengine.desktop.Helpers;

public static class FfmpegHelper
{
    /*
    *ConvertAsync will return the location of the converted file if successful, or null if unsuccesful
    **/
    public async static Task<string> ConvertAsync(string file, double startAt = 0, double endAt = 5, GifQuality quality = GifQuality.q1080p, int fps = 60 )
    {
        try
        {
              string home = Environment.GetEnvironmentVariable("HOME");

            //if file is not a video, dont bother converting. Just return the image.
            if(Path.GetExtension(file).ToLower() != ".mp4" && Path.GetExtension(file).ToLower() != ".mkv"){
                Console.WriteLine($"Extension is {Path.GetExtension(file)}");
                string copyTo = home + "/Pictures/wallpapers/" + file.Split("/").Last();
                File.Copy(file,copyTo,true);
                if(!File.Exists(copyTo))
                    return null;
                File.Delete(file);
                return copyTo;
            }
            string convertTo = home + "/Pictures/wallpapers/" + file.Split("/").Last().Split(".").First() + ".gif";
            string ffmpegArgs =
                $" -ss {startAt} -t {endAt} -i \"{file}\" -vf \"scale=-1:{QualityParser(quality)}:flags=lanczos,fps={fps}\" -loop 0 -y \"{convertTo}\"";
            var convertProcess = new Process
            {
                StartInfo = new()
                {
                    FileName = "ffmpeg",
                    Arguments = ffmpegArgs,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            // convertProcess.OutputDataReceived += (sender, args) =>
            // {
            //         Debug.WriteLine($"Received Output: {args.Data}");
            // };
            // convertProcess.ErrorDataReceived += (sender, errorArgs) =>
            // {
            //     if (errorArgs.Data != null)
            //     {
            //         Debug.WriteLine($"Received Error: {errorArgs.Data}");
            //     }
            // };
            convertProcess.Start();
            convertProcess.BeginOutputReadLine();
            convertProcess.BeginErrorReadLine();
            convertProcess.WaitForExit();
            //if gif does not exist then conversion failed. Return false
            if (!File.Exists(convertTo))
            {
                return null;
            }
            //if everything went smoothly, delete the mp4.
           File.Delete(file);
            return convertTo;
        }
        catch
        {
            return null;
        }
      
    }

    private static string QualityParser(GifQuality quality)
    {
        switch (quality)
        {
            case GifQuality.q360p:
                return "360";
                break;
            case GifQuality.q480p:
                return "480";
                break;
            case GifQuality.q720p:
                return "720";
                break;
            case GifQuality.q1080p:
                return "1080";
                break;
            case GifQuality.q1440p:
                return "1440";
                break;
            case GifQuality.q2160p:
                return "2160";
                break;
            default:
                return "1080";
                break;
        }
    }
}
