using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using LibVLCSharp.Shared;
using swengine.desktop.Helpers;
using swengine.desktop.Models;
using swengine.desktop.Services;

namespace swengine.desktop.ViewModels;

public partial class ApplyWindowViewModel : ViewModelBase
{
    private WallpaperResponse _wallpaperResponse;
    private MotionBgsService _motionBgsService = new();
    private readonly LibVLC _libVlc = new LibVLC("--input-repeat=2");

    public ApplyWindowViewModel()
    {
        MediaPlayer = new MediaPlayer(_libVlc);
    }
    public MediaPlayer MediaPlayer { get; }
    
    [ObservableProperty] private Wallpaper wallpaper;
    public WallpaperResponse WallpaperResponse
    {
        get { return _wallpaperResponse;}
        set { SetProperty(ref _wallpaperResponse,value);
            ObjectCreated();
        }
    }
    public async void ObjectCreated()
    {
        //Debug.WriteLine(WallpaperResponse.Src);
         Wallpaper = await _motionBgsService.InfoAsync(WallpaperResponse.Src,Title:WallpaperResponse.Title);
        using var media = new Media(_libVlc, new Uri(Wallpaper.Preview));
        MediaPlayer.Play(media);
    }

    public async void ApplyWallpaper()
    {
        if (Wallpaper != null)
        {
            Debug.WriteLine("Began Downloading Wallpaper");
           bool downloadResult =  await DownloadHelper.DownloadAsync(Wallpaper.DownloadLink, Wallpaper.Title);
           if (downloadResult)
           {
               Debug.WriteLine("Download complete. Began converting wallpaper");
               //begin conversion with result of download
               string prospectiveFile = Environment.GetEnvironmentVariable("HOME") +
                                        "/Pictures/wallpapers/preconvert/" + Wallpaper.Title + ".mp4";
             
           await FfmpegHelper.ConvertAsync(prospectiveFile, 0, 5,
               GifQuality.q1080p);
           }
            Debug.WriteLine("Application complete");
        }
     
    }
}

public class DesignApplyWindowViewModel : ApplyWindowViewModel
{
    public DesignApplyWindowViewModel()
    {
        Wallpaper = new()
        {
            Title = "Garp With Galaxy Impact",
            SourceFile = "https://www.motionbgs.com/media/6384/garp-with-galaxy-impact.960x540.mp4",
            WallpaperType = WallpaperType.Live,
            Resolution = "Resolution\":\"3840x2160"
        };
        
    }
}
