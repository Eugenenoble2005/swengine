using System.Collections.Generic;
using System.Threading.Tasks;
using swengine.desktop.Models;
using swengine.desktop.Scrapers;
namespace swengine.desktop.Services;

public class WallpapersClanService : IBgsProvider
{
  public async Task<Wallpaper> InfoAsync(string Query, string Title = "")
  {
    try
    {
      return await WallpapersClanScraper.InfoAsync(Query);
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

      return await WallpapersClanScraper.LatestAsync(Page);
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

      return await WallpapersClanScraper.SearchAsync(Query, Page);
    }
    catch
    {
      return default;
    }
  }
}
