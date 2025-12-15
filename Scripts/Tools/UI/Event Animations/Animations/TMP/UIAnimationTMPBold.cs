using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIAnimationTMPBold : UIAnimationBase
{
    [SerializeField] private bool valueFrom = false;
    [SerializeField] private bool valueTo = true;
    [Space]
    [SerializeField] private bool setToFromOnAwake = true;
    
    private TMP_Text _tmpText;
    
    protected override float OnEvent(bool forward)
    {
        this.DOKill();
        
        _tmpText.fontStyle = GetFontStyle(forward ? valueTo : valueFrom);
        
        return 0f;
    }

    private void Awake()
    {
        _tmpText = GetComponent<TMP_Text>();
        if (setToFromOnAwake)
        {
            _tmpText.fontStyle = GetFontStyle(valueFrom);
        }
    }

    private FontStyles GetFontStyle(bool value)
    {
        return value ? FontStyles.Bold : FontStyles.Normal;
    }
}
