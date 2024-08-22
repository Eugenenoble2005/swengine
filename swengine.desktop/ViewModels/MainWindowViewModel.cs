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
    //current page
    [ObservableProperty] private int currentPage = 1;
    [ObservableProperty] private List<WallpaperResponse> wallpaperResponses;
    [ObservableProperty] private bool dataLoading = false;
    async void GetWallpapers()
    {
        DataLoading = true;
        WallpaperResponses = await _motionBgsService.LatestAsync(Page: CurrentPage);
        DataLoading = false;
    }

    public void Paginate(string seek)
    {
        if (seek == "up")
        {
            CurrentPage++;
        }
        else if (seek == "down" && CurrentPage > 1)
        {
            CurrentPage--;
        }
        GetWallpapers();
    }
}