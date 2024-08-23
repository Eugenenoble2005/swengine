using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using HtmlAgilityPack;
using swengine.desktop.Models;

namespace swengine.desktop.Scrapers;

public static class MoewallsScraper
{
    private static readonly string MoewallsBase = "https://www.moewalls.com";
    public async static Task<string> LatestAsync(int Page = 1)
    {
        List<WallpaperResponse> wallpaper_responses = new();
        using (var http = new HttpClient())
        {
            string url = MoewallsBase + $"/page/{Page}";
            var request = await http.GetAsync(url);
            if (request.IsSuccessStatusCode)
            {
                var response = await request.Content.ReadAsStringAsync();
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);
                var g1_collection_item = htmlDoc.DocumentNode.SelectNodes("//li[contains(@class,'g1-collection-item')]");
                foreach (var item in g1_collection_item)
                {
                    try
                    {
                        string img_src = item.SelectSingleNode(".//img").GetAttributeValue("src", null);
                        string title = item.SelectSingleNode(".//a[@class='g1-frame']").GetAttributeValue("title", null);
                        string src = item.SelectSingleNode(".//a[@class='g1-frame']").GetAttributeValue("href", null);
                        wallpaper_responses.Add(new()
                        {
                            Title = title,
                            Src = src,
                            Thumbnail = img_src
                        });
                    }
                   catch{}
                }

                return JsonSerializer.Serialize(wallpaper_responses);
            }
        }

        return default;
    }

    public async static Task<string> InfoAsync(string Query, string Title)
    {
        using (var http = new HttpClient())
        {
            var request = await http.GetAsync(Query);
            if (request.IsSuccessStatusCode)
            {
                string response = await request.Content.ReadAsStringAsync();
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);
                string source_tag = htmlDoc.DocumentNode.SelectSingleNode("//source[@type='video/mp4']")
                    .GetAttributeValue("src", null);
                string download = "https://go.moewalls.com/download.php?video=" +htmlDoc.DocumentNode.SelectSingleNode("//button[@id='moe-download']")
                    .GetAttributeValue("data-url", null);
                string text_xs = "4k";
                return JsonSerializer.Serialize(
                    new Wallpaper()
                    {
                        Title = Title,
                        Resolution = text_xs,
                        Preview = source_tag,
                        WallpaperType = WallpaperType.Live,
                        SourceFile = download,
                        NeedsReferrer = true
                    }
                );
            }
        }

        return default;
    }
}