using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;
using swengine.desktop.Models;
using swengine.desktop.Services;

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
    public string[] Providers => new[] { "Motionbgs.com", "Moewalls.com" }; 
    
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

    public async void OpenUploadDialog(){
        ContentDialog uploadDialog = new(){
            Title = "Upload a wallpaper to your desktop",
            Content = UploadDialogContent(),
            PrimaryButtonText = "Upload",
            IsPrimaryButtonEnabled = true
        };
        await uploadDialog.ShowAsync();
    }
    private object UploadDialogContent(){
        StackPanel panel = new();
        TextBlock header = new()
        {
            Text = "Upload file or URL",
            FontSize = 20,

        };

        TextBlock orText = new(){
            Text = "Or",
            Margin = new(0,10,0,10)
        };
        TextBox urlBox = new(){
             Watermark = "Enter url"
        };

        Button uploadFile = new(){
            Content = "Upload File"
        };
        
        panel.Children.Add(header);

        panel.Children.Add(urlBox);

        panel.Children.Add(orText);

        panel.Children.Add(uploadFile);
        return panel;
    }
}