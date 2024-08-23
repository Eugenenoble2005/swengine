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
            Assert.IsInstanceOfType<WallpaperResponse>(latest);
        }
    }
}