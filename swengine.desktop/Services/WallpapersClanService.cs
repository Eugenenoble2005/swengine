using System.Collections.Generic;
using System.Threading.Tasks;
using swengine.desktop.Models;
using swengine.desktop.Scrapers;
namespace swengine.desktop.Services;

public class WallpapersClanService : IBgsProvider
{
  public Task<Wallpaper> InfoAsync(string Query, string Title = "")
  {
    try
    {
      return WallpapersClanScraper.InfoAsync(Query);
    }
    catch
    {
      return default;
    }
  }
  public Task<List<WallpaperResponse>> LatestAsync(int Page = 1)
  {
    try
    {

      return WallpapersClanScraper.LatestAsync(Page);
    }
    catch
    {
      return default;
    }
  }

  public Task<List<WallpaperResponse>> SearchAsync(string Query, int Page = 1)
  {
    try
    {

      return WallpapersClanScraper.SearchAsync(Query, Page);
    }
    catch
    {
      return default;
    }
  }
}
