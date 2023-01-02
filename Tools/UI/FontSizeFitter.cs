using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FontSizeFitter : MonoBehaviour
{
    private static Dictionary<string, float> minFontSizes;

    [SerializeField] private string _key;

    private Text _text;
    private TextMeshProUGUI _tmp;


    private void Start()
    {
        _text = GetComponent<Text>();
        _tmp = GetComponent<TextMeshProUGUI>();

        if (minFontSizes == null) 
        {
            minFontSizes = new Dictionary<string, float>();
        }

        StartCoroutine(WriteFontSizeCoroutine());
        StartCoroutine(ReadFontSizeCoroutine());
    }

    private void SetActiveAutoSize(bool arg) 
    {
        if (_text != null)
        {
            _text.resizeTextForBestFit = arg;
        }
        else if (_tmp != null)
        {
            _tmp.enableAutoSizing = arg;
        }
    }

    private IEnumerator WriteFontSizeCoroutine()
    {
        SetActiveAutoSize(true);

        yield return null;

        float fontSize = 0;
        if (_text != null)
        {
            fontSize = _text.cachedTextGenerator.fontSizeUsedForBestFit;
        }
        else if (_tmp != null)
        {
            fontSize = _tmp.fontSize;
        }

        SetActiveAutoSize(false);

        if (minFontSizes.ContainsKey(_key))
        {
            minFontSizes[_key] = Mathf.Min(minFontSizes[_key], fontSize);
        }
        else 
        {
            minFontSizes.Add(_key, fontSize);
        }
    }

    private IEnumerator ReadFontSizeCoroutine() 
    {
        yield return null;
        yield return null;

        if (_text != null)
        {
            _text.fontSize = (int)minFontSizes[_key];
        }
        else if (_tmp != null)
        {
            _tmp.fontSize = minFontSizes[_key];
        }
    }
}
