using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LevelCounter : MonoBehaviour
{
    [SerializeField] private SideType side = SideType.Right;
    [SerializeField] private int addToCount;

    [InjectService] private ILevelManagerService _levelService;
    
    private void Start()
    {
        ServiceLocator.Default.InjectServicesInComponent(this);
        StartCoroutine(AddNumberCoroutine());
    }

    private IEnumerator AddNumberCoroutine()
    {
        yield return null;
        
        TextMeshProUGUI tmp = GetComponent<TextMeshProUGUI>();
        int level = _levelService.GetCurrentLevelCount() + addToCount;
        tmp.text = side == SideType.Right ? $"{tmp.text} {level}" : $"{level} {tmp.text}";
    }

    private enum SideType
    {
        Left,
        Right
    }
}
