using System.Diagnostics;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public abstract class FileExplorerProcessor
{
    public static FileExplorerProcessor Instance
    {
        get
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            return new FileExplorerProcessorWindows();
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            return new FileExplorerProcessorMacos();
#elif UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
            return new FileExplorerProcessorLinux();
#endif
            return null;
        }
    }

    public abstract void Open(string path);
}

public class FileExplorerProcessorWindows : FileExplorerProcessor
{
    public override void Open(string path)
    {
        System.Diagnostics.Process.Start("explorer.exe","/select,"+path);
    }
}

public class FileExplorerProcessorMacos : FileExplorerProcessor
{
    public override void Open(string path)
    {
        Process.Start("open", path);
    }
}

public class FileExplorerProcessorLinux : FileExplorerProcessor
{
    public override void Open(string path)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "xdg-open",
            Arguments = $"{path}",
            UseShellExecute = true,
            RedirectStandardOutput = false,
            RedirectStandardError = false,
        });
    }
}