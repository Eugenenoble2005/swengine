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
    public async static Task ApplyWallpaperAsync(Wallpaper wallpaper,  ApplicationStatusWrapper applicationStatusWrapper, GifQuality selectedResolution, string selectedFps,CancellationToken token, string referrer = null)
    {
        if (token.IsCancellationRequested)
        {
            Debug.WriteLine("Cancellation requested");
            return;
        }
        if (wallpaper != null)
        {
            Debug.WriteLine("Began Downloading Wallpaper");
             applicationStatusWrapper.Status = "Downloading Wallpaper...";
            bool downloadResult =  await DownloadHelper.DownloadAsync(wallpaper.SourceFile, wallpaper.Title, wallpaper.NeedsReferrer,referrer  );
            if (downloadResult)
            {
                if (token.IsCancellationRequested)
                {
                    Debug.WriteLine("Cancellation requested");
                    return;
                }
                Dispatcher.UIThread.Post(() =>
                {
                    applicationStatusWrapper.Status  = "Download complete. Converting Wallpaper...";
                });
                Debug.WriteLine("Download complete. Began converting wallpaper");
                //begin conversion with result of download
                string prospectiveFile = Environment.GetEnvironmentVariable("HOME") +
                                         "/Pictures/wallpapers/preconvert/" + wallpaper.Title + ".mp4";
                //very dangerous with the int.Parse(). Must refine this
                bool convertResult =  await FfmpegHelper.ConvertAsync(prospectiveFile, 0, 5,
                    selectedResolution,fps:int.Parse(selectedFps));
                if (convertResult)
                {
                    if (token.IsCancellationRequested)
                    {
                        Debug.WriteLine("Cancellation requested");
                        return;
                    }
                    Dispatcher.UIThread.Post(() =>
                    {
                        applicationStatusWrapper.Status  = "Conversion complete. Applying wallpaper. This might take a while depending on the details of your wallpaper...";
                    });
                    Debug.WriteLine("Conversion Complete. Began Apply wallpaper");
                    await SwwwHelper.ApplyAsync(Environment.GetEnvironmentVariable("HOME") +
                                                "/Pictures/wallpapers/" + wallpaper.Title + ".gif");
                    Dispatcher.UIThread.Post(() =>
                    {
                        applicationStatusWrapper.Status  = "Wallpaper Applied Successfully";
                    });
                }
                else
                {
                    /*******
                     *              HANDLE FAILED CONVERSION is convertResult is false
                     */
                    Dispatcher.UIThread.Post(() =>
                    {
                        applicationStatusWrapper.Status = "An error occured while converting. Please try again";
                    });
                }
                    
            }
            else
            {
                /*******
                 *              HANDLE FAILED DOWNLOAD if downloadResult is false
                 */
                Dispatcher.UIThread.Post(() =>
                {
                    applicationStatusWrapper.Status = "An error occured while dowloading. Please try again";
                });
            }
            Debug.WriteLine("Application complete");
        }
       
    }
}