using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace swengine.desktop.Helpers;

public static class DownloadHelper
{
    /*
    *DownloadAsync will return the location of the downloaded file if successful, or null if unsuccesful
    **/
    public static async Task<string> DownloadAsync(string Link,string Title, bool NeedsReferrer = false, string Referer = null)
    {
        string HOME = Environment.GetEnvironmentVariable("HOME");
        try
        {
            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, Link);
            Debug.WriteLine(Link);
            if (NeedsReferrer)
            {
    
                //if provider requires referer
                request.Headers.Referrer = new Uri(Referer);
                Debug.WriteLine(Referer);
            }
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            using var s = await response.Content.ReadAsStreamAsync();
            string extension = ".mp4"; // default extension
            if (response.Content.Headers.ContentDisposition != null)
            {
                var filename = response.Content.Headers.ContentDisposition.FileName?.Trim('\"');
                if (!string.IsNullOrEmpty(filename))
                {
                    // Extract the file extension from the filename
                    extension = Path.GetExtension(filename);
                }
            }
            //create preconvert directory to store raw MP4s before being passed over to FFMPEG
            Directory.CreateDirectory(HOME + "/Pictures/wallpapers/preconvert");
            using var fs = new FileStream($"{HOME}/Pictures/wallpapers/preconvert/{Title}{extension}", FileMode.Create);
            await s.CopyToAsync(fs);
            //if file does not exist in path then download failed. Return false
            if (!File.Exists($"{HOME}/Pictures/wallpapers/preconvert/{Title}{extension}"))
            {
                return null;
            }
            return $"{HOME}/Pictures/wallpapers/preconvert/{Title}{extension}";
        }
        catch
        {
            #if __DEBUG__
            throw;
            #endif
            return null;
        }
     
    }

}