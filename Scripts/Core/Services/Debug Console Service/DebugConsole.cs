using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugConsole : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private List<ConsoleModule> modules;
    [Space] 
    [SerializeField] private Button buttonPrefab;
    [SerializeField] private Transform buttonsRoot;

    private DebugConsoleService _service;
    
    private void Start()
    {
        _service = GetComponentInParent<DebugConsoleService>();
        closeButton.onClick.AddListener(Close);
        
        SpawnButtons();
        OnModuleButtonClicked(modules[0]);
    }

    private void SpawnButtons()
    {
        foreach (ConsoleModule module in modules)
        {
            Button button = Instantiate(buttonPrefab, buttonsRoot);
            button.GetComponentInChildren<TextMeshProUGUI>().text = module.Name;
            button.onClick.AddListener(() => OnModuleButtonClicked(module));
        }        
    }

    private void OnModuleButtonClicked(ConsoleModule module)
    {
        foreach (ConsoleModule m in modules)
        {
            m.Root.SetActive(m == module);
        }
    }

    private void Close()
    {
        _service.CloseConsole();
    }

    [Serializable]
    private class ConsoleModule
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public GameObject Root { get; private set; }
    }
}
