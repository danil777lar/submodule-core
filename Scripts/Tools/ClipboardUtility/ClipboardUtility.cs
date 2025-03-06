using UnityEngine;

public static class ClipboardUtility
{
    public static bool ClipboardAvailable => GetProcessor() != null;
    
    public static void CopyToClipboard(string text)
    {
        ClipboardProcessor processor = GetProcessor();
        if (processor != null)
        {
            processor.Copy(text);
        }
    }
    
    private static ClipboardProcessor GetProcessor()
    {
        #if UNITY_EDITOR
        return new ClipboardProcessorEditor();
        #endif
        
        return null;
    }
}
