namespace swengine.desktop.Models;

/***
 * This represents the response from searches and indexes. 
 */
public class WallpaperResponse
{
    /***
     * The title of the wallpaper as provided by the provider
     */
    public string? Title { get; set; }

    /**
     * The static image preview that will be displayed in searches
     */
    public string? Thumbnail { get; set; }

    /**
     * This is the link to the main page of the wallpaper. The page that contains information about the paper including the high quality download link
     */
    public string? Src { get; set; }

}
public class Wallpaper
{
    /**
     * Title of the wallpaper, 
     */
    public string? Title { get; set; }

    /**
     * MP4 preview of the wallpaper. This is not the source of the wallpaper as it is very low quality. This will be displayed in libvlc so the user knows what the wallpaper looks like before they download it
     */
    public string? Preview { get; set; }

    /**
     * Type of the wallpaper, whether Live or static. I initially planned to add providers for static wallpapers. Maybe later on
     */
    public WallpaperType? WallpaperType { get; set; }

    /**
     * Link to the high quality mp4 gif file which will be downloaded, converted by ffmpeg to a high quality gif and  applied by the swwww daemon
     */
    public string? SourceFile { get; set; }

    /**
     * Resolution of the wallpaper as provider by the provider. Pretty much useless
     */
    public string? Resolution { get; set; }

    /**
     * Whether or not the download requires an http referrer header to be passed for it to be downloaded. If true, the download helper will send the required referrer to allow the download
     */
    public bool NeedsReferrer { get; set; } = false;
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