using UnityEngine;

public class ClipboardProcessorEditor : ClipboardProcessor
{
    public override void Copy(string text)
    {
        GUIUtility.systemCopyBuffer = text;
    }
}
