using System.Diagnostics;
using System.Threading.Tasks;

namespace swengine.desktop.Helpers;

public static class SwwwHelper
{
    public async static Task<bool> ApplyAsync(string file)
    {
        try
        {
            var applyProcess = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "swww",
                    Arguments = $"img \"{file}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            applyProcess.OutputDataReceived += (sender, args) => { Debug.WriteLine($"Received Output: {args.Data}"); };
            applyProcess.ErrorDataReceived += (sender, errorArgs) =>
            {
                if (errorArgs.Data != null)
                {
                    Debug.WriteLine($"Received Error: {errorArgs.Data}");
                }
            };
            applyProcess.Start();
            applyProcess.BeginErrorReadLine();
            applyProcess.BeginOutputReadLine();
            applyProcess.WaitForExit();
            Process.Start("notify-send \"Wallpaper succesfully applied!\"");
            return true;
        }
        catch
        {
            return false;
        }
    }
}