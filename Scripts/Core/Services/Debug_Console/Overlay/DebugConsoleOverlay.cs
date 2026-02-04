using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugConsoleOverlay : MonoBehaviour
{
    [SerializeField] private TMP_Text entryPrefab;

    private List<TMP_Text> _textInstances = new List<TMP_Text>();
    private List<OverlayEntry> _entries = new List<OverlayEntry>(); 

    public void AddEntry(OverlayEntry entry)
    {
        if (!_entries.Contains(entry))
        {
            _entries.Add(entry);
        }
    }

    public void RemoveEntry(OverlayEntry entry)
    {
        if (_entries.Contains(entry))
        {
            _entries.Remove(entry);
        }
    }

    public void ClearEntries()
    {
        _entries.Clear();
    }

    private void Start()
    {
        entryPrefab.gameObject.SetActive(false);

        AddEntry(new OverlayEntryFps());
        AddEntry(new OverlayEntryBatches());
        AddEntry(new OverlayEntryTriangles());
    }

    private void Update()
    {
        CheckTextsCount();

        for (int i = 0; i < _entries.Count; i++)
        {
            _textInstances[i].text = _entries[i].GetData();
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
