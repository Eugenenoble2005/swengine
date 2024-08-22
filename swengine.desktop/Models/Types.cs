namespace swengine.desktop.Models;

//Response from searches
public class WallpaperResponse
{
    public string? Title { get; set; }
    public string? Thumbnail { get; set; }
    
    //link to the page that has information about the wallpaper
    public string? Src { get; set; }
    
}
public class Wallpaper
{
    //Title of wallpaper
    public string? Title { get; set; }
   
    //Preview for wallpapaer, usually a scaled down mp4
    public string? Preview { get; set; }
    public WallpaperType? WallpaperType { get; set; }
    
    //Link to download wallpaper
    public string? SourceFile { get; set; }
    
    //source resolution
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
    q1080p,
    q1440p,
    q2160p,
}