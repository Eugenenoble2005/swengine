using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using swengine.desktop.Models;
using swengine.desktop.Services;

namespace swengine.desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        GetWallpapers();
    }
    private MotionBgsService _motionBgsService = new();

    [ObservableProperty] private List<WallpaperResponse> wallpaperResponses;
    
    async void GetWallpapers()
    {
        WallpaperResponses = await _motionBgsService.LatestAsync(Page: 1);
        Debug.WriteLine("testing function");
       
    }
}