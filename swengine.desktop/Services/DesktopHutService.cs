using System.Collections.Generic;
using System.Threading.Tasks;
using swengine.desktop.Models;
using swengine.desktop.Scrapers;

namespace swengine.desktop.Services;

public class DesktopHutService : IBgsProvider {
    public async Task<Wallpaper> InfoAsync(string Query, string Title = "") {
        try {
            return await DesktopHutScraper.InfoAsync(Query);
        } catch {
            return default;
        }
    }

    public async Task<List<WallpaperResponse>> LatestAsync(int Page = 1) {
        try {
            return await DesktopHutScraper.LatestOrSearchAsync(Page, "latest");
        } catch {
            return default;
        }
    }

    public async Task<List<WallpaperResponse>> SearchAsync(string Query, int Page = 1) {
        try {
            return await DesktopHutScraper.LatestOrSearchAsync(Page, "search", Query);
        } catch {
            return default;
        }
    }
}
