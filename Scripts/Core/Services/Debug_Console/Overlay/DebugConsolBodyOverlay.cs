using Larje.Core.Services.DebugConsole;
using UnityEngine;
using UnityEngine.UI;

public class DebugConsolBodyOverlay : MonoBehaviour
{
    [SerializeField] private Toggle overlayActiveToggle;
    [SerializeField] private Slider textTransparencySlider;

    private DebugConsoleService _debugConsoleService;

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
