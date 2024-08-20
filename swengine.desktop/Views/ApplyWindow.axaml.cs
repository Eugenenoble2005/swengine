using Avalonia;
using Avalonia.Controls;
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
    }
}