using System;
using System.IO;

namespace swengine.desktop.Helpers;
public static class CustomScriptsHelper
{
    public static readonly string scripts_location = Environment.GetEnvironmentVariable("HOME") != null ? Path.Join(Environment.GetEnvironmentVariable("HOME"),".swengine_after_run.sh") : null;

    /**
    *Write content of Content to the scripts file
    */
    public static bool SetScriptsFileContent(String content){
        try{
             //AvaloniaEdit TextEditor accounts for lines automatically, i dont need to bother reading and writing individual lines.
            File.WriteAllText(scripts_location,content);
            return true;
        }
        catch{
            return false;
        }
    }

    public static string? ScriptsFileContent {
            get { 
                if(!File.Exists(scripts_location))
                    return null;
                //AvaloniaEdit TextEditor accounts for lines automatically, i dont need to bother reading and writing individual lines.
                return File.ReadAllText(scripts_location);
             }
    }
}