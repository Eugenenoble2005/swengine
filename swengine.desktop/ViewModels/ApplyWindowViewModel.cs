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
    //get search results from previous window
    private WallpaperResponse _wallpaperResponse;
    
    //MotionBgs service
    public IBgsProvider BgsProvider { get; set; }
    //Resolution user selected. Defaults to 4k.
    [ObservableProperty] private GifQuality selectedResolution = GifQuality.q2160p;
    
    //duration selected by user
    [ObservableProperty] private int selectedDuration = 5;
    //FPS user selected for GIF
    [ObservableProperty] private string selectedFps = "60";
    
    //Binding that determines if the video in the window is visible. Drawing over NativeControlHost is not very easy in avalonia so we must hide the video whenever we want to display a ContentDialog
    [ObservableProperty] private bool isVideoVisible = true;
    
    //ApplicationStatus as in Status of applying the wallpaper. It is wrapped in a mutable class so it can be passed as a reference to the WallpaperHelper
    [ObservableProperty] private ApplicationStatusWrapper applicationStatusWrapper = new();
    
    //Initialize Native Libvlc client for playing the wallpaper preview
    private readonly LibVLC _libVlc = new LibVLC("--input-repeat=2");

    public ApplyWindowViewModel()
    {
        MediaPlayer = new MediaPlayer(_libVlc);
    }
    
    //Media Player object for libvlc
    public MediaPlayer MediaPlayer { get; }
    
    
    //The wallpaper object that will be gotten from the Bg service after it has obtained information about the wallpaper.
    [ObservableProperty] private Wallpaper wallpaper;
    public WallpaperResponse WallpaperResponse
    {
        get { return _wallpaperResponse;}
        set { SetProperty(ref _wallpaperResponse,value);
            ObjectCreated();
        }
    }
    
    //Called when the WallpaperResponse object is set while the window is opening
    public async void ObjectCreated()
    {
      
         Wallpaper = await BgsProvider.InfoAsync(WallpaperResponse.Src,Title:WallpaperResponse.Title);
         // Debug.WriteLine(JsonSerializer.Serialize(Wallpaper));
        using var media = new Media(_libVlc, new Uri(Wallpaper.Preview));
        MediaPlayer.Play(media);
    }

    //Apply wallpaper. Will be abstracted for Both Live and static wallpaper
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
                    WallpaperHelper.ApplyWallpaperAsync(Wallpaper,  ApplicationStatusWrapper, SelectedResolution, SelectedFps,SelectedDuration,
                        ctx.Token, referrer:WallpaperResponse.Src);
                });
            };
            applicationStatusDialog.Closed += (sender, args) =>
            {
                //try to cancel the wallpaper application process. I haven't gotten this quite right yet as it only gets an opportunity to cancel after each step of the application. Will revisit.
                Debug.WriteLine("Attempting to cancel");
                ctx.Cancel();
            };
            await applicationStatusDialog.ShowAsync();
         

        }
      
    }

    //Content for the content dialog that requests for FPS,resolution, e.t.c
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
        TextBox FpsSelector = new(){ };
        FpsSelector.Bind(TextBox.TextProperty, new Binding()
        {
            Path = "SelectedFps",
            Source = this,
            Mode = BindingMode.TwoWay
        });

        TextBlock DurationSelectorText = new() { Text = "Select duration" };
        TextBox DurationSelector = new();
        DurationSelector.Bind(TextBox.TextProperty, new Binding()
        {
            Path = "SelectedDuration",
            Source=this,
            Mode = BindingMode.TwoWay
        });
        ToolTip.SetTip(DurationSelector,new TextBlock()
        {
            Text = "5 seconds is usually sufficient for wallpapers. Longer wallpapers would be larger in size and take longer to apply"
        });
        panel.Children.Add(ResolutionSelectorText);
        panel.Children.Add(ResolutionSelector);
        
        panel.Children.Add(FpsSelectorText);
        panel.Children.Add(FpsSelector);
        
        panel.Children.Add(DurationSelectorText);
        panel.Children.Add(DurationSelector);
        
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

//wrapper class for ApplicationStatus
public partial class ApplicationStatusWrapper : ObservableObject
{
    [ObservableProperty] private string status;
}