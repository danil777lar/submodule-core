using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIAnimationTMPColor : UIAnimationBase
{
    [SerializeField] private float duration = 0.25f;
    [Space]
    [SerializeField] private Color valueFrom = Color.white;
    [SerializeField] private Color valueTo = Color.white;
    [Space]
    [SerializeField] private bool setToFromOnAwake = true;
    
    private TMP_Text _tmpText;
    
    protected override float OnEvent(bool forward)
    {
        this.DOKill();
        
        _tmpText.DOColor(forward ? valueTo : valueFrom, duration)
            .SetUpdate(true)
            .SetTarget(this);
        
        return duration;
    }

    private void Awake()
    {
        _tmpText = GetComponent<TMP_Text>();
        if (setToFromOnAwake)
        {
            _tmpText.color = valueFrom;
        }
    }
}
