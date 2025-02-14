using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class CanvasScaleFactorFitter : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve;
    
    private CanvasScaler _canvasScaler;
    
    private void Start()
    {
        _canvasScaler = GetComponent<CanvasScaler>();
    }
    
    private void Update()
    {
        float ratio = (float)Screen.width / (float)Screen.height;
        _canvasScaler.matchWidthOrHeight = curve.Evaluate(ratio);
    }
}
