using System;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;
using LibVLCSharp.Shared;
using swengine.desktop.Helpers;
using swengine.desktop.Models;
using swengine.desktop.Services;

namespace swengine.desktop.ViewModels;

public partial class ApplyWindowViewModel : ViewModelBase
{
    private WallpaperResponse _wallpaperResponse;
    private MotionBgsService _motionBgsService = new();
    
    [ObservableProperty] private GifQuality selectedResolution = GifQuality.q2160p;
    [ObservableProperty] private string selectedFps = "60";
    [ObservableProperty] private bool isVideoVisible = true;
    [ObservableProperty] private string applicationStatus;
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
        //dialog cannot draw over video, so hide video when dialog is about to display
        IsVideoVisible = false;
        ContentDialog dialog = new()
        {
            Title = "Apply this wallpaper",
            PrimaryButtonText = "Apply",
            IsPrimaryButtonEnabled = true,
            Content = ApplyDialogContent()
        };
        dialog.Closed += (sender, args) =>
        {
            //show the video again when dialog is closing
            IsVideoVisible = true;
        };
        var dialogResponse = await dialog.ShowAsync();
        if (dialogResponse == ContentDialogResult.Primary)
        {
            dialog.Hide();
           await Task.Delay(1000);
            IsVideoVisible = false; 
            var applicationStatusDialog = new ContentDialog()
            {
                Title = "Applying Wallpaper",
                CloseButtonText = "Stop"
            };
            applicationStatusDialog.Bind(ContentDialog.ContentProperty, new Binding()
            {
                Path = "ApplicationStatus",
                Source = this,
                Mode = BindingMode.TwoWay,
            });
            applicationStatusDialog.Closed += (sender, args) =>
            {
                IsVideoVisible = true;
            };
            applicationStatusDialog.ShowAsync();
            CancellationTokenSource ctx = new();
            Task.Run(() =>
            {
                ApplyWallpaperAsync(ctx.Token);
            });
            applicationStatusDialog.Closed += (sender, args) =>
            {
                Debug.WriteLine("Attempting to cancel");
                ctx.Cancel();
            };

        }
      
    }

    private object ApplyDialogContent()
    {
        StackPanel panel = new();
        //resolution selector
        ComboBox ResolutionSelector = new();
        TextBlock ResolutionSelectorText = new() { Text = "Select Resolution:" };
        ResolutionSelector.ItemsSource = new[] { GifQuality.q2160p, GifQuality.q1440p, GifQuality.q1080p, GifQuality.q720p, GifQuality.q480p, GifQuality.q360p};
        ResolutionSelector.Bind(ComboBox.SelectedItemProperty, new Binding()
        {
            Path = "SelectedResolution",
            Source = this,
            Mode = BindingMode.TwoWay
        });
        
        //FPS selector
        TextBlock FpsSelectorText = new() { Text = "Select Frames per second:" };
        TextBox FpsSelector = new();
        FpsSelector.Bind(TextBox.TextProperty, new Binding()
        {
            Path = "SelectedFps",
            Source = this,
            Mode = BindingMode.TwoWay
        });
        panel.Children.Add(ResolutionSelectorText);
        panel.Children.Add(ResolutionSelector);
        
        panel.Children.Add(FpsSelectorText);
        panel.Children.Add(FpsSelector);
        
        return panel;
    }
    
   private async Task ApplyWallpaperAsync(CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            Debug.WriteLine("Cancellation requested");
            return;
        }
        if (Wallpaper != null)
        {
            Debug.WriteLine("Began Downloading Wallpaper");
             ApplicationStatus = "Downloading Wallpaper...";
            bool downloadResult =  await DownloadHelper.DownloadAsync(Wallpaper.DownloadLink, Wallpaper.Title);
            if (downloadResult)
            {
                if (token.IsCancellationRequested)
                {
                    Debug.WriteLine("Cancellation requested");
                    return;
                }
                Dispatcher.UIThread.Post(() =>
                {
                    ApplicationStatus = "Download complete. Converting Wallpaper...";
                });
                Debug.WriteLine("Download complete. Began converting wallpaper");
                //begin conversion with result of download
                string prospectiveFile = Environment.GetEnvironmentVariable("HOME") +
                                         "/Pictures/wallpapers/preconvert/" + Wallpaper.Title + ".mp4";
                //very dangerous with the int.Parse(). Must refine this
                bool convertResult =  await FfmpegHelper.ConvertAsync(prospectiveFile, 0, 5,
                    SelectedResolution,fps:int.Parse(SelectedFps));
                if (convertResult)
                {
                    if (token.IsCancellationRequested)
                    {
                        Debug.WriteLine("Cancellation requested");
                        return;
                    }
                    Dispatcher.UIThread.Post(() =>
                    {
                        ApplicationStatus = "Conversion complete. Applying wallpaper. This might take a while...";
                    });
                    Debug.WriteLine("Conversion Complete. Began Apply wallpaper");
                    await SwwwHelper.ApplyAsync(Environment.GetEnvironmentVariable("HOME") +
                                                "/Pictures/wallpapers/" + Wallpaper.Title + ".gif");
                    Dispatcher.UIThread.Post(() =>
                    {
                        ApplicationStatus = "Wallpaper Applied Successfully";
                    });
                }
                    
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
