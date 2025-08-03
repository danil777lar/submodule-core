using UnityEngine;

public static class LarjeSystemUtility
{
    public static bool ClipboardAvailable => ClipboardProcessor.Instance != null;
    
    public static void CopyToClipboard(string text)
    {
        ClipboardProcessor processor = ClipboardProcessor.Instance;
        if (processor != null)
        {
            processor.Copy(text);
        }
    }
    
    public static void OpenFileExplorer(string text)
    {
        
    }
}
