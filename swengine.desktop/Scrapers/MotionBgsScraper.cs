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
    public static async Task<string> Latest(int Page)
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
}

