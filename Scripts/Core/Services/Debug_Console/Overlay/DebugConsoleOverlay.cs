using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugConsoleOverlay : MonoBehaviour
{
    [SerializeField] private TMP_Text entryPrefab;

    private int frames;
    private float time;
    private float fps;

    private List<TMP_Text> _textInstances = new List<TMP_Text>();
    private List<Func<string>> _entries = new List<Func<string>>(); 

    private void Start()
    {
        entryPrefab.gameObject.SetActive(false);

        _entries.Add(() => 
        {    
            frames++;
            time += Time.unscaledDeltaTime;

            if (time >= 0.5f)
            {
                fps = frames / time;
                frames = 0;
                time = 0;
            }

            return $"FPS: {Mathf.Round(fps).ToString()}";
        });
    }

    private void Update()
    {
        CheckTextsCount();

        for (int i = 0; i < _entries.Count; i++)
        {
            _textInstances[i].text = _entries[i]?.Invoke();
        }
    }

    private void CheckTextsCount()
    {
        while (_entries.Count > _textInstances.Count)
        {
            TMP_Text instance = Instantiate(entryPrefab, entryPrefab.transform.parent);
            instance.gameObject.SetActive(true);

            _textInstances.Add(instance);
        }

        while (_entries.Count < _textInstances.Count)
        {
            _textInstances.RemoveAt(0);
        }
    }
}
