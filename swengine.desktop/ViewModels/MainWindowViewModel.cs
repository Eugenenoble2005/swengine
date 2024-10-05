using System;
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
        SearchRun += SearchRun;
    }

    public IBgsProvider BgsProvider;
    public string[] Providers => new[] { "Motionbgs.com", "Moewalls.com","Mylivewallpapers.com", "Wallhaven.cc" , "Wallpaperscraft.com" }; 
    public string[] Backends => new[] {"SWWW","PLASMA","GNOME"};

    private bool _appendingToInfinteScroll = false;
    
    private string _selectedProvider = "Motionbgs.com";
    private string _selectedBackend = "SWWW";
    public event EventHandler<EventArgs> SearchRun;

    private int _infiniteScrollPage = 1;
    public string SelectedProvider
    {
        get => _selectedProvider;
        set
        {
            SetProperty(ref _selectedProvider, value);
            SetProvider();
        }
    }
    public string SelectedBackend {
        get => _selectedBackend;
        set {
            SetProperty(ref _selectedBackend,value);
            SetBackend();
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
    [ObservableProperty] public bool infinteScrollLoading = false;
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
            case "Mylivewallpapers.com":
                BgsProvider = new MyLiveWallpapersService();
                break;
            case "Wallpaperscraft.com":
                BgsProvider = new WallpapersCraftService();
                break;
            default:
                break;
        }
        //changing provider should reset _infinteScrollPage to the current page
        _infiniteScrollPage = CurrentPage;
        _appendingToInfinteScroll  = false;
        Search();
    }
    private void SetBackend(){
        
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
        _infiniteScrollPage = CurrentPage;
        _appendingToInfinteScroll = false;
        Search();
        SearchRun.Invoke(this, EventArgs.Empty);
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

public async void AppendToInfinteScroll()
{
    try{
    if(_appendingToInfinteScroll) return;
    _appendingToInfinteScroll = true;
        InfinteScrollLoading = true;

        if (SearchTerm.Length == 0)
        {
            foreach (var response in await BgsProvider?.LatestAsync(_infiniteScrollPage + 1))
            {
                // add new empty search results to the itemsRepeater
                if (response != null)
                {
                    WallpaperResponses.Add(response);
                }
            }
            _infiniteScrollPage++;
            _appendingToInfinteScroll = false;
            InfinteScrollLoading = false;
            return;
        }

        foreach (var response in await BgsProvider?.SearchAsync(SearchTerm, _infiniteScrollPage + 1))
        {
            // add new search results to the itemsRepeater
            if (response != null)
            {
                WallpaperResponses.Add(response);
            }
        }
        _infiniteScrollPage++;
        InfinteScrollLoading = false;
        _appendingToInfinteScroll = false;
        return;
        }
        catch{
            InfinteScrollLoading = false;
            _appendingToInfinteScroll = false;
        }
    }
  }
    

