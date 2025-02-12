using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.Tools;
using TMPro;

public static class TextMeshProUGUIExtensions
{
    public static Tween DOTextLinear(this TextMeshProUGUI tmp, string targetText, float duration, bool removeExtraSpaces = true)
    {
        string initialText = tmp.text;
        Tween tween = DOTween.To(() => 0f, t =>
        {
            string result = initialText.LerpLinear(targetText, t);
            if (removeExtraSpaces)
            {
                result = result.RemoveExtraSpaces();
            }
            tmp.text = result;
        }, 1f, duration);

        return tween;
    }
}
