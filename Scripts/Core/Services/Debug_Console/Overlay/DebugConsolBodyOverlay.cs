using System.Collections.Generic;
using Larje.Core.Services.DebugConsole;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugConsolBodyOverlay : MonoBehaviour
{
    [SerializeField] private Slider textTransparencySlider;
    [SerializeField] private Toggle overlayActiveToggle;
    [Space]
    [SerializeField] private Toggle debugGroupTogglePrefab;

    private DebugConsoleService _debugConsoleService;
    private Dictionary<string, Toggle> _debugGroupToggles = new Dictionary<string, Toggle>();

    private void Start()
    {
        _debugConsoleService = GetComponentInParent<DebugConsoleService>();

        overlayActiveToggle.isOn = _debugConsoleService.OverlayActive;
        overlayActiveToggle.onValueChanged.AddListener(OnOverlayToggleChanged);

        textTransparencySlider.minValue = 0f;
        textTransparencySlider.maxValue = 1f;
        textTransparencySlider.value = _debugConsoleService.OverlayTextTransparency;
        textTransparencySlider.onValueChanged.AddListener(OnTextTransparencyChanged);

        _debugConsoleService.Overlay.SetTextTransparency(_debugConsoleService.OverlayTextTransparency);

        debugGroupTogglePrefab.gameObject.SetActive(false);
    }

    private void Update()
    {
        foreach (string g in LarjeDebug.Overlay.GetGroups())
        {
            if (!_debugGroupToggles.ContainsKey(g))
            {
                Toggle newToggle = Instantiate(debugGroupTogglePrefab, debugGroupTogglePrefab.transform.parent);
                newToggle.gameObject.SetActive(true);
                newToggle.isOn = LarjeDebug.Overlay.IsDebugGroupEnabled(g);
                newToggle.GetComponentInChildren<TMP_Text>().text = g;
                newToggle.onValueChanged.AddListener((bool value) => { LarjeDebug.Overlay.SetDebugGroupEnabled(g, value); });
                _debugGroupToggles.Add(g, newToggle);
            }
        }
    }

    private void OnOverlayToggleChanged(bool arg)
    {
        _debugConsoleService.OverlayActive = arg;
    }

    private void OnTextTransparencyChanged(float value)
    {
        _debugConsoleService.OverlayTextTransparency = value;
    }
}
