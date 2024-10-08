using Newtonsoft.Json;
using swengine.desktop.Models;
using swengine.desktop.Scrapers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace swengine.tests.Scrapers;

[TestClass]
public class MotionBgsTests
{
    [TestMethod]
    public async Task LatestTest()
    {
        for (int i = 0; i <= 10; i++)
        {
            var latest = JsonSerializer.Deserialize<List<WallpaperResponse>>(await MotionBgsScraper.LatestAsync(i));
            
            Assert.IsNotNull(latest);
            Assert.IsInstanceOfType<List<WallpaperResponse>>(latest);
        }
    }

    [TestMethod]
    public async Task InfoTest()
    {
      
            var latest = JsonSerializer.Deserialize<List<WallpaperResponse>>(await MotionBgsScraper.LatestAsync(1));
            for (int j = 0; j < latest.Count; j++)
            {
                var info = MotionBgsScraper.InfoAsync(latest[j].Src, latest[j].Title);
                var info_object = JsonSerializer.Deserialize<Wallpaper>(await info);
                Assert.IsNotNull(info_object);
                Assert.IsInstanceOfType<Wallpaper>(info_object);
        }
    }

    [TestMethod]
    public async Task SearchTest()
    {
        string[] search_terms = new[] { "nature", "ocean", "space", "city" };
        foreach (var searchTerm in search_terms)
        {
            var search = JsonSerializer.Deserialize<List<WallpaperResponse>>(await MotionBgsScraper.SearchAsync(searchTerm,1));
            Assert.IsNotNull(search);
            Assert.IsInstanceOfType<List<WallpaperResponse>>(search);
        }
       
    }
}