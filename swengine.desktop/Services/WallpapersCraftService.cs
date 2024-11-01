using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using swengine.desktop.Models;

namespace swengine.desktop.Services;
public class WallpapersCraftService : IBgsProvider
{
    public async Task<Wallpaper> InfoAsync(string Query, string Title = "")
    {
        try
        {
            return await Scrapers.WallpapersCraftScraper.InfoAsync(Query);
        }
        catch
        {
            return default;
        }
    }

    public async Task<List<WallpaperResponse>> LatestAsync(int Page = 1)
    {

        try
        {
            return await Scrapers.WallpapersCraftScraper.LatestOrSearchAsync(page: Page, function: "latest");
        }
        catch
        {
            return default;
        }
    }

    public async Task<List<WallpaperResponse>> SearchAsync(string Query, int Page = 1)
    {
        try
        {
            return await Scrapers.WallpapersCraftScraper.LatestOrSearchAsync(page: Page, function: "search", query: Query);
        }
        catch
        {
            return default;
        }
    }
}
