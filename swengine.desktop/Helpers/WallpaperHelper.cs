using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using swengine.desktop.Models;
using swengine.desktop.ViewModels;

namespace swengine.desktop.Helpers;

public static class WallpaperHelper
{
    public async static Task ApplyWallpaperAsync
    (
    Wallpaper wallpaper, 
     ApplicationStatusWrapper applicationStatusWrapper,
    GifQuality selectedResolution,
     string selectedFps, 
    int selectedDuration,
    bool bestSettings, 
    string backend,
    CancellationToken token, 
    string referrer = null
    
    )
    {
        if(wallpaper == null){
            return;
        }
        long CURRENT_TIMESTAMP = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        applicationStatusWrapper.Status = "Downloading Wallpaper...";
        string downloadResult =  await DownloadHelper.DownloadAsync(wallpaper.SourceFile, wallpaper.Title, wallpaper.NeedsReferrer,referrer  );
        //if download failed, return and notify user.
        if(downloadResult == null){
            Dispatcher.UIThread.Post(() =>
                {
                    applicationStatusWrapper.Status = "An error occured while dowloading. Please try again.";
            });
            return;
        }
        //if cancelled
         if (token.IsCancellationRequested)
        {
            Debug.WriteLine("Cancellation requested");
            return;
        }
        Dispatcher.UIThread.Post(() =>
         {
            applicationStatusWrapper.Status  = "Download complete. Converting Wallpaper...";
        });

        /**
        *       Download complete begin conversion
        */

           //very dangerous with the int.Parse(). Must refine this
        string convertResult =  await FfmpegHelper.ConvertAsync(downloadResult, 0, selectedDuration,selectedResolution,fps:int.Parse(selectedFps),bestSettings);
        //if conversion failed, return and notify user
        if(convertResult == null){
             Dispatcher.UIThread.Post(() =>
            {
                applicationStatusWrapper.Status = "An error occured while converting. Please try again.";
            });
            return;
        }
        //if canceled
         if (token.IsCancellationRequested)
        {
            Debug.WriteLine("Cancellation requested");
            return;
        }
        Dispatcher.UIThread.Post(() =>{
                applicationStatusWrapper.Status  = "Conversion complete. Applying wallpaper. This might take a while depending on the details of your wallpaper...";
        });


        /**
        *       Conversion complete begin application
        */
        
         await SwwwHelper.ApplyAsync(convertResult,backend);
         long APPLICATION_TIME = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - CURRENT_TIMESTAMP;
         TimeSpan applicationTimeSpan = TimeSpan.FromSeconds(APPLICATION_TIME);
         string applicationTimeSpanText = "";
         applicationTimeSpanText = applicationTimeSpan.TotalMinutes > 1 ? $"{applicationTimeSpan.TotalMinutes.ToString("F2")} minute(s)" : $"{applicationTimeSpan.TotalSeconds.ToString("F2")} second(s)";
        
        Dispatcher.UIThread.Post(() =>
        {
            applicationStatusWrapper.Status  = $"Wallpaper Applied Successfully in {applicationTimeSpanText}";
        });
       
    }
}
