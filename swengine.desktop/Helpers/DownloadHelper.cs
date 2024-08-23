using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace swengine.desktop.Helpers;

public static class DownloadHelper
{
    public static async Task<bool> DownloadAsync(string Link,string Title, bool NeedsReferer = false, string Referer = null)
    {
        string HOME = Environment.GetEnvironmentVariable("HOME");
        try
        {
            
            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, Link);
            if (NeedsReferer)
            {
                //if provider requires referer
                // Debug.WriteLine(Referer);
                request.Headers.Referrer = new Uri(Referer);
            }
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            using var s = await response.Content.ReadAsStreamAsync();

          
            //create preconvert directory to store raw MP4s before being passed over to FFMPEG
            Directory.CreateDirectory(HOME + "/Pictures/wallpapers/preconvert");
            using var fs = new FileStream($"{HOME}/Pictures/wallpapers/preconvert/{Title}.mp4", FileMode.Create);
            await s.CopyToAsync(fs);
            //if file does not exist in path then download failed.
            if (!File.Exists($"{HOME}/Pictures/wallpapers/preconvert/{Title}.mp4"))
            {
                return false;
            }
            return true;
        }
        catch
        {
            return false;
        }
     
    }
}