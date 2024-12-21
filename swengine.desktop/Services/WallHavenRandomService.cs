using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using swengine.desktop.Models;

namespace swengine.desktop.Services;
/*I'm doing this because there's no reason to believe other providers have random listings. No need to implement it in the contract*/
public class WallHavenRandomService : IBgsProvider
{
    public async Task<Wallpaper> InfoAsync(string Query, string Title = "")
    {

        try
        {
            return await Scrapers.WallHavenScraper.InfoAsync(Query);
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

            return await Scrapers.WallHavenScraper.LatestOrSearchAsync(page: Page, Function: "random");
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

            return await Scrapers.WallHavenScraper.LatestOrSearchAsync(page: Page, Function: "search", Query: Query);
        }
        catch
        {
            return default;
        }
    }
}