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
using AvaloniaEdit;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;
using swengine.desktop.Models;
using swengine.desktop.Services;
using swengine.desktop.Views;
namespace swengine.desktop.ViewModels;
public partial class MainWindowViewModel{
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
    private object CustomScriptsDialogContent(){
         TextBlock customScriptText = new(){
            Text = "Enter commands you want to run after a new wallpaper has been set. One command per line. Take great caution here",
            TextWrapping = Avalonia.Media.TextWrapping.Wrap
        };
          TextEditor customScriptBox = new(){
           WordWrap = true,
           ShowLineNumbers = true,  
           Height = 100
        };
        // TextBox customScriptBox = new(){
        //     Height=100,
        //     Watermark="Enter commands to run, substitute $wallpaper for the newely set wallpaper",
        //     AcceptsReturn = true,
        //     TextWrapping = Avalonia.Media.TextWrapping.Wrap
        // };
        StackPanel wrapper = new();
        wrapper.Children.Add(customScriptText);
        wrapper.Children.Add(customScriptBox);

        return wrapper;
    }
    public async void OpenCustomScriptsDialog(){
        var dialog  =  new ContentDialog(){
                Title = "Custom Scripts",
                IsPrimaryButtonEnabled = true,
                PrimaryButtonText = "Add Script",
                Content = CustomScriptsDialogContent()
        };
        await dialog.ShowAsync();
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