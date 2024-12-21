using System.Collections.Generic;
using System.Threading.Tasks;

using swengine.desktop.Models;

namespace swengine.desktop.Services;

public class PexelsVideoService : IBgsProvider
{
    Task<Wallpaper> IBgsProvider.InfoAsync(string Query, string Title)
    {
        throw new System.NotImplementedException();
    }

    Task<List<WallpaperResponse>> IBgsProvider.LatestAsync(int Page)
    {
        throw new System.NotImplementedException();
    }

    Task<List<WallpaperResponse>> IBgsProvider.SearchAsync(string Query, int Page)
    {
        throw new System.NotImplementedException();
    }
}
