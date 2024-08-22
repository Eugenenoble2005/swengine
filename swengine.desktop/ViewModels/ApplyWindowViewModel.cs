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
    //wrapping in a class so it can be passed by reference
    [ObservableProperty] private ApplicationStatusWrapper applicationStatusWrapper = new();
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
                Path = "ApplicationStatusWrapper.Status",
                Source = this,
                Mode = BindingMode.TwoWay,
            });
            applicationStatusDialog.Closed += (sender, args) =>
            {
                IsVideoVisible = true;
            };
            CancellationTokenSource ctx = new();
            applicationStatusDialog.Opened += (sender, args) =>
            {
                Task.Run(() =>
                {
                    WallpaperHelper.ApplyWallpaperAsync(Wallpaper,  ApplicationStatusWrapper, SelectedResolution, SelectedFps,
                        ctx.Token);
                });
            };
            applicationStatusDialog.Closed += (sender, args) =>
            {
                Debug.WriteLine("Attempting to cancel");
                ctx.Cancel();
            };
            await applicationStatusDialog.ShowAsync();
         

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
    
}

public class DesignApplyWindowViewModel : ApplyWindowViewModel
{
    public DesignApplyWindowViewModel()
    {
        Wallpaper = new()
        {
            Title = "Garp With Galaxy Impact",
            Preview = "https://www.motionbgs.com/media/6384/garp-with-galaxy-impact.960x540.mp4",
            WallpaperType = WallpaperType.Live,
            Resolution = "Resolution\":\"3840x2160"
        };
        
    }
}

public partial class ApplicationStatusWrapper : ObservableObject
{
    [ObservableProperty] private string status;
}