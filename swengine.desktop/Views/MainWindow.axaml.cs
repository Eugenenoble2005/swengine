using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Input;
using swengine.desktop.Models;
using swengine.desktop.ViewModels;

namespace swengine.desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ScrollViewer.ScrollChanged += (object sender, ScrollChangedEventArgs args)=>{
          var scrollview = sender as ScrollViewer;
          var scrollHeight = (int) scrollview.Extent.Height - scrollview.Viewport.Height;
        // > 100 to prevent calling this method when the scrollview is empty
         if(scrollview.Offset.Y == scrollHeight && scrollHeight > 100 ){

            //append to infinte scroll
            (DataContext as MainWindowViewModel).AppendToInfinteScroll();
          }
          (DataContext as MainWindowViewModel).SearchRun += (s,e)=>{
                //scroll up when user paginates
                scrollview.ScrollToHome();
        };
        };
    }

    private async void OpenApplyWindow(object? sender, TappedEventArgs e)
    {
        var dc = (DataContext as MainWindowViewModel);
        WallpaperResponse Tag = (WallpaperResponse)(sender as StackPanel).Tag;
        if (Tag == null)
        {
            return;
        }
        var applyWindow = new ApplyWindow()
        {
            DataContext = new ApplyWindowViewModel()
            {
                BgsProvider = (DataContext as MainWindowViewModel).BgsProvider,
                Backend = (DataContext as MainWindowViewModel).SelectedBackend,
                WallpaperResponse = Tag,
                //pass the current provider so the Apply window knows which provider to query for the wallpaper
               
            }
        };
        var result = await applyWindow.ShowDialog<ApplyWindowViewModel?>(this);
    }
}
