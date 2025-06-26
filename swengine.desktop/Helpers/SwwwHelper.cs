using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace swengine.desktop.Helpers;

/**
Used for all backends including KDE and GNOME and not just swww
*/

public static class SwwwHelper {
    public async static Task<bool> ApplyAsync(string file, string backend) {
        try {
            var applyProcess = new Process() {
                StartInfo = ApplyProcessStartInfo(backend, file)
            };
            applyProcess.OutputDataReceived += (sender, args) => { Debug.WriteLine($"Received Output: {args.Data}"); };
            applyProcess.ErrorDataReceived += (sender, errorArgs) => {
                if (errorArgs.Data != null) {
                    Debug.WriteLine($"Received Error: {errorArgs.Data}");
                }
            };
            applyProcess.Start();
            applyProcess.BeginErrorReadLine();
            applyProcess.BeginOutputReadLine();
            applyProcess.WaitForExit();


            //send notification
            Process.Start(new ProcessStartInfo() {
                FileName = "notify-send",
                Arguments = "\"Wallpaper set succesfully\"",
                UseShellExecute = false,
                CreateNoWindow = true,
            });

            //run custom scripts asynchronously, basically a fire and forget
            if (File.Exists(CustomScriptsHelper.scripts_location)) {
                Task.Run(() => {
                    string script_location = CustomScriptsHelper.scripts_location;
                    //export wallpaper variable then run the user's script
                    string command = $"\"{script_location}\" \"\"{file}\"\"";

                    //first make script executable
                    Process.Start(new ProcessStartInfo() {
                        FileName = "chmod",
                        Arguments = $"+x {script_location}",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });

                    var scriptProcess = new Process() {
                        StartInfo = new() {
                            FileName = "/bin/bash",
                            Arguments = $"-c \"{command}\"",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true,
                        }
                    };

                    scriptProcess.Start();

                });
            }
            return true;
        } catch {
            return false;
        }
    }

    private static ProcessStartInfo ApplyProcessStartInfo(string backend, string file) {
        string filename = null;
        string arguments = null;
        switch (backend) {
            case "SWWW":
                filename = "swww";
                arguments = $"img \"{file}\"";
                break;
            case "YIN":
                filename = "yinctl";
                arguments = $"--img \"{file}\"";
                break;
            case "PLASMA":
                filename = "plasma-apply-wallpaperimage";
                arguments = $"\"{file}\"";
                break;
            case "GNOME":
                filename = "gsettings";
                arguments = $"set org.gnome.desktop.background picture-uri \"{file}\"";
                break;

        }
        Console.WriteLine(filename + " " + arguments);
        return new() {
            FileName = filename,
            Arguments = arguments,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
    }
}
