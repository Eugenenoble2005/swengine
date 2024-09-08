using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data;
using Avalonia.Platform.Storage;
using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;
using swengine.desktop.Models;
using swengine.desktop.Services;
using swengine.desktop.Views;

namespace swengine.desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        //set provider initially to motionbgs.com
        SetProvider();
        Search();
    }

    public IBgsProvider BgsProvider;
    public string[] Providers => new[] { "Motionbgs.com", "Moewalls.com", "Wallhaven.cc" }; 
    
    private string _selectedProvider = "Motionbgs.com";

    public string SelectedProvider
    {
        get => _selectedProvider;
        set
        {
            SetProperty(ref _selectedProvider, value);
            SetProvider();
        }
    }
   [ObservableProperty] private string searchTerm = "";
   
    //current page
    [ObservableProperty] private int currentPage = 1;
    [ObservableProperty] private List<WallpaperResponse> wallpaperResponses;
    [ObservableProperty] private bool dataLoading = false;

    [ObservableProperty] private string selectedFile = null;

    [ObservableProperty] private TextDocument customScriptsContent = new(){
        Text = ""
    };
    
    // async void GetWallpapers()
    // {
    //     DataLoading = true;
    //     WallpaperResponses = await BgsProvider.LatestAsync(Page: CurrentPage);
    //     DataLoading = false;
    // }

    private void SetProvider()
    {
        switch (SelectedProvider)
        {
            case "Motionbgs.com":
                BgsProvider = new MotionBgsService();
                break;
            case "Moewalls.com":
                BgsProvider = new MoewallsService();
                
                break;
            case "Wallhaven.cc":
                BgsProvider = new WallHavenService();
                break;
            default:
                break;
        }
        Search();
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
        DataLoading = true;
        if (SearchTerm.Length == 0)
        {
            //empty search
            WallpaperResponses = await BgsProvider.LatestAsync(Page: CurrentPage);
            DataLoading = false;
            return;
        }
        WallpaperResponses = await BgsProvider.SearchAsync(SearchTerm, CurrentPage);
        DataLoading = false;
    }


}