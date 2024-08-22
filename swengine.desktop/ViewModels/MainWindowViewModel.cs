using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using Avalonia.Controls;
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

    //Later on will try to use reflection to try and dynamically load classes that implement IBgsService
    public string[] Providers => new[] { "Motionbgs.com" };
    [ObservableProperty] private string selectedProvider = "Motionbgs.com";
    private readonly MotionBgsService _motionBgsService = new();

   [ObservableProperty] private string searchTerm = "";
   
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
        Search();
    }
    public async void Search()
    {
        if (SearchTerm.Length == 0)
        {
            //empty search
            GetWallpapers();
            return;
        }
        DataLoading = true;
        WallpaperResponses = await _motionBgsService.SearchAsync(SearchTerm, CurrentPage);
       
        DataLoading = false;
    }
}