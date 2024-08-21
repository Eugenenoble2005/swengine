using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using LibVLCSharp.Avalonia;
using swengine.desktop.ViewModels;

namespace swengine.desktop.Views;

public partial class ApplyWindow : Window
{
    public ApplyWindow()
    {
        InitializeComponent();
        video.Loaded += ((sender, args) =>
        {
            var datacontext = DataContext as ApplyWindowViewModel;
           video.MediaPlayer = datacontext.MediaPlayer;
        });
        Closed += (sender, args) =>
        {
            //stop all players
           ( DataContext as ApplyWindowViewModel).MediaPlayer.Stop();
          
        };
    }

    private void ApplyWallpaper(object? sender, RoutedEventArgs e)
    {
        var dataContext = DataContext as ApplyWindowViewModel;
        
    }
}
