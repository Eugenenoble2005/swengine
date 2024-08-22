using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using swengine.desktop.Models;

namespace swengine.desktop.Services;

public class MotionBgsService : IBgsProvider
{
    public async Task<List<WallpaperResponse>> LatestAsync(int Page)
    {
        try
        {
            return JsonSerializer.Deserialize<List<WallpaperResponse>>(
                    await Scrapers.MotionBgsScraper.LatestAsync(Page));
        }
        catch
        {
            return default;
        }
    }

    public async Task<Wallpaper> InfoAsync(string Query, string Title = "")
    {
        try
        {
            return JsonSerializer.Deserialize<Wallpaper>(
                await Scrapers.MotionBgsScraper.InfoAsync(Query,Title));
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
            return JsonSerializer.Deserialize<List<WallpaperResponse>>(await Scrapers.MotionBgsScraper.SearchAsync(Query,Page));
        }
        catch
        {
            return default;
        }
    }
}