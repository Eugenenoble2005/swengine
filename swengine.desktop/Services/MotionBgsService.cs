using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using swengine.desktop.Models;

namespace swengine.desktop.Services;

public class MotionBgsService : IBgsProvider
{
    public async Task<List<WallpaperResponse>> Latest(int Page)
    {
        try
        {
            return JsonSerializer.Deserialize<List<WallpaperResponse>>(
                    await Scrapers.MotionBgsScraper.Latest(Page));
        }
        catch
        {
            return default;
        }
    }
}