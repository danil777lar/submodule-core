using UnityEngine;

public abstract class ClipboardProcessor
{
    public static ClipboardProcessor Instance
    {
        get
        {
#if UNITY_EDITOR
            return new ClipboardProcessorEditor();
#endif
            return null;
        }
    }

    public abstract void Copy(string text);
}

public class ClipboardProcessorEditor : ClipboardProcessor
{
    public override void Copy(string text)
    {
        GUIUtility.systemCopyBuffer = text;
    }
}