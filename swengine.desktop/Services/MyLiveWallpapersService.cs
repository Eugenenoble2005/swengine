using System.Collections.Generic;
using System.Threading.Tasks;
using swengine.desktop.Models;

namespace swengine.desktop.Services;
public class MyLiveWallpapersService : IBgsProvider
{
    public Task<Wallpaper> InfoAsync(string Query, string Title = "")
    {
        throw new System.NotImplementedException();
    }

    public Task<List<WallpaperResponse>> LatestAsync(int Page = 1)
    {
        throw new System.NotImplementedException();
    }

    public Task<List<WallpaperResponse>> SearchAsync(string Query, int Page = 1)
    {
        throw new System.NotImplementedException();
    }
}
