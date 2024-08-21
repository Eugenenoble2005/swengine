namespace swengine.desktop.Models;

public class WallpaperResponse
{
    public string? Title { get; set; }
    public string? Thumbnail { get; set; }
    public string? Src { get; set; }
    
}
public class Wallpaper
{
    public string? Title { get; set; }
    
    public string? SourceFile { get; set; }
    
    public string? Preview { get; set; }
    public WallpaperType? WallpaperType { get; set; }
    
    public string? DownloadLink { get; set; }
    public string? Resolution { get; set; }
}

public enum WallpaperType
{
    Live,
    Static,
}

public enum GifQuality
{
    q360p,
    q480p,
    q720p,
    q1080p
}