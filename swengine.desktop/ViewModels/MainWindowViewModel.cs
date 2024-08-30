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

    public async void OpenUploadDialog(){
        ContentDialog uploadDialog = new(){
            Title = "Upload a wallpaper to your desktop",
            Content = UploadDialogContent(),
            PrimaryButtonText = "Upload",
            IsPrimaryButtonEnabled = true
        };
      var result =   await uploadDialog.ShowAsync();
      if(result == ContentDialogResult.Primary){
        if(SelectedFile != null){
            var applyWindow = new ApplyWindow()
                {
                    DataContext = new ApplyWindowViewModel()
                    {
                        BgsProvider = new LocalBgService(),
                        WallpaperResponse = new(){
                             Src = SelectedFile,
                             Thumbnail = null,
                             Title = SelectedFile
                        },
                        //pass the current provider so the Apply window knows which provider to query for the wallpaper
                    
                    }
                };
                await applyWindow.ShowDialog<ApplyWindowViewModel>((Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow);
        }
      }

    }
    private object UploadDialogContent(){
        StackPanel panel = new();
        Button uploadFile = new(){
            Content = "Upload File"
        };
        TextBlock selectedFile = new();
        selectedFile.Bind(TextBlock.TextProperty,new Binding(){
            Source = this,
            Path = "SelectedFile",
            Mode = BindingMode.TwoWay
        });
        uploadFile.Click += (s,e)=>{
            handleFileDialog();
        };
        panel.Children.Add(uploadFile);
        panel.Children.Add(selectedFile);
        return panel;
    }

    private async void handleFileDialog(){
        var toplevel = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow;

        // Start async operation to open the dialog.
        var files = await toplevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Text File",
            AllowMultiple = false,
             FileTypeFilter = new[] { new FilePickerFileType("filesv") {Patterns = new List<string>(){
                "*.jpg", "*.jpeg", "*.png", "*.bmp","*.mp4","*.MP4","*.mkv","*.MKV","*.gif"
             }} }
        });

        if (files.Count >= 1)
            SelectedFile = files[0].TryGetLocalPath();
    }
}