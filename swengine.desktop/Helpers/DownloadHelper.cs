using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace swengine.desktop.Helpers;

public static class DownloadHelper
{
    public static async Task<bool> DownloadAsync(string Link,string Title)
    {
        string HOME = Environment.GetEnvironmentVariable("HOME");
        try
        {
            using var client = new HttpClient();
            using var s = await client.GetStreamAsync(Link);
            //create preconvert directory to store raw MP4s before being passed over to FFMPEG
            Directory.CreateDirectory(HOME + "/Pictures/wallpapers/preconvert");
            using var fs = new FileStream($"{HOME}/Pictures/wallpapers/preconvert/{Title}.mp4", FileMode.Create);
            await s.CopyToAsync(fs);
            return true;
        }
        catch
        {
            return false;
        }
     
    }
}