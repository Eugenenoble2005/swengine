using System.Collections.Generic;
using System.Diagnostics;
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
                WallpaperResponse = Tag,
                //pass the current provider so the Apply window knows which provider to query for the wallpaper
               
            }
        };
        var result = await applyWindow.ShowDialog<ApplyWindowViewModel?>(this);
    }
}
