using Newtonsoft.Json;
using swengine.desktop.Models;
using swengine.desktop.Scrapers;
using JsonSerializer = System.Text.Json.JsonSerializer;
[TestClass]
public class MoewallsTest
{
    [TestMethod]
    public async Task LatestTest()
    {
        for (int i = 0; i <= 10; i++)
        {
            var latest = JsonSerializer.Deserialize<List<WallpaperResponse>>(await MoewallsScraper.LatestOrSearchAsync(i,"latest"));
            
            Assert.IsNotNull(latest);
            Assert.IsInstanceOfType<List<WallpaperResponse>>(latest);
        }
    }

    [TestMethod]
    public async Task InfoTest()
    {
      
        var latest = JsonSerializer.Deserialize<List<WallpaperResponse>>(await MoewallsScraper.LatestOrSearchAsync(1,Function:"latest"));
        for (int j = 0; j < latest.Count; j++)
        {
            var info = MoewallsScraper.InfoAsync(latest[j].Src, latest[j].Title);
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
            var search = JsonSerializer.Deserialize<List<WallpaperResponse>>(await MoewallsScraper.LatestOrSearchAsync(1,"search",searchTerm));
            Assert.IsNotNull(search);
            Assert.IsInstanceOfType<List<WallpaperResponse>>(search);
        }
       
    }
}