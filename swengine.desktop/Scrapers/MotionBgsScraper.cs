using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using HtmlAgilityPack;
using swengine.desktop.Models;

namespace swengine.desktop.Scrapers;

public static class MotionBgsScraper
{
    public static async Task<string> LatestAsync(int Page)
    {
        string url = $"https://motionbgs.com/hx2/latest/{Page}/";
        using (var http = new HttpClient())
        {
            var request = await http.GetAsync(url);
            if (request.IsSuccessStatusCode)
            {
                List<WallpaperResponse> wallpaper_response = new();
                var string_response = await request.Content.ReadAsStringAsync();
                HtmlDocument htmlDoc = new();
                htmlDoc.LoadHtml(string_response);
                var a_links = htmlDoc.DocumentNode.SelectNodes("//a");
                foreach (var aLink in a_links)
                {
                    string img_src =  "https://www.motionbgs.com" + aLink.SelectSingleNode(".//img").GetAttributeValue("src","");
                    string title = aLink.SelectSingleNode(".//span[@class='ttl']").InnerHtml;
                    string src = "https://www.motionbgs.com" +  aLink.GetAttributeValue("href", "");
                   wallpaper_response.Add(new()
                   {
                       Title = title,
                       Src = src,
                       Thumbnail = img_src
                   });
                }
                return JsonSerializer.Serialize(wallpaper_response);
            } 
        }
        return default;
    }

    public static async Task<string> InfoAsync(string Query, string Title)
    {
        using (var http = new HttpClient())
        {
            var request = await http.GetAsync(Query);
            if (request.IsSuccessStatusCode)
            {
                string response = await request.Content.ReadAsStringAsync();
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);

                string source_tag = "https://www.motionbgs.com" + htmlDoc.DocumentNode.SelectSingleNode("//source[@type='video/mp4']")
                    .GetAttributeValue("src", null);
                string text_xs =
                    htmlDoc.DocumentNode.SelectSingleNode("//div[@class='text-xs']").InnerHtml.Split(" ")[0];
                string download = "https://www.motionbgs.com" + htmlDoc.DocumentNode.SelectSingleNode("//div[@class='download']")
                    .SelectSingleNode(".//a").GetAttributeValue("href", null);
                return JsonSerializer.Serialize(
                    new Wallpaper()
                    {
                        Title = Title,
                        Resolution = text_xs,
                        Preview = source_tag,
                        WallpaperType = WallpaperType.Live,
                        DownloadLink = download
                    }
                );
            }
        }

        return default;
    }
}

