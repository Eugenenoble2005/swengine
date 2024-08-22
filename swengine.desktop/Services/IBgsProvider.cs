using System.Collections.Generic;
using System.Threading.Tasks;
using swengine.desktop.Models;

namespace swengine.desktop.Services;

public interface IBgsProvider
{
    public Task<List<WallpaperResponse>> LatestAsync(int Page=1);
    public Task<Wallpaper> InfoAsync(string Query, string Title = "");
    public Task<List<WallpaperResponse>> SearchAsync(string Query, int Page = 1);
}