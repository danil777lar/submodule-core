using Larje.Core.Services.DebugConsole;
using UnityEngine;
using UnityEngine.UI;

public class DebugConsolBodyOverlay : MonoBehaviour
{
    [SerializeField] private Toggle overlayActiveToggle;

    private DebugConsoleService _debugConsoleService;

    private void Start()
    {
        _debugConsoleService = GetComponentInParent<DebugConsoleService>();
        overlayActiveToggle.isOn = _debugConsoleService.OverlayActive;        
        overlayActiveToggle.onValueChanged.AddListener(OnOverlayToggleChanged);
    }

    private void OnOverlayToggleChanged(bool arg)
    {
        _debugConsoleService.OverlayActive = arg;
    }
}
