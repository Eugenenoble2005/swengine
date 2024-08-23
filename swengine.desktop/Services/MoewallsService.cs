using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using swengine.desktop.Models;
using swengine.desktop.Scrapers;

namespace swengine.desktop.Services;

public class MoewallsService : IBgsProvider
{
    public async Task<List<WallpaperResponse>> LatestAsync(int Page = 1)
    {
        try
        {
            return JsonSerializer.Deserialize<List<WallpaperResponse>>(await MoewallsScraper.LatestAsync(Page));
        }
        catch
        {
            return default;
        }
    }

    public async Task<Wallpaper> InfoAsync(string Query, string Title = "")
    {
       
            return JsonSerializer.Deserialize<Wallpaper>(await MoewallsScraper.InfoAsync(Query,Title));

    }
    public async Task<List<WallpaperResponse>> SearchAsync(string Query, int Page = 1)
    {
        return default;
    }
}