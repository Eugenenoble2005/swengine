using System.Collections.Generic;
using System.Threading.Tasks;
using swengine.desktop.Models;

namespace swengine.desktop.Services;

public interface IBgsProvider
{
    public Task<List<WallpaperResponse>> Latest(int Page);
}