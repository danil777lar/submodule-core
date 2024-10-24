using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SpriteAnimation : MonoBehaviour
{
    [SerializeField] private float frameDuration;
    [SerializeField] private Sprite[] sprites;
    private int _framesChanged;
    private Image _image;
    
    private void Start()
    {
        StartCoroutine(AnimationCoroutine());
        _image = GetComponent<Image>();
    }

    private IEnumerator AnimationCoroutine()
    {
        yield return new WaitForSecondsRealtime(frameDuration);
        _framesChanged++;
        _image.sprite = sprites[_framesChanged % sprites.Length];
        StartCoroutine(AnimationCoroutine());
    }
}