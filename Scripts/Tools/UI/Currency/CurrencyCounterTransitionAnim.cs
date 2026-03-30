using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Larje.Core;
using Larje.Core.Services;
using ProjectConstants;
using TMPro;
using UnityEngine;

public class CurrencyCounterTransitionAnim : MonoBehaviour
{
    [SerializeField] private CurrencyCounter currencyCounter;
    [Space]
    [SerializeField] private Transform targetPoint;
    [SerializeField] private GameObject animPrefab;
    [Space]
    [SerializeField] private string leftModificator;
    [SerializeField] private string rightModificator;

    [Header("Animation")]
    [SerializeField] private float spawnDuration = 0.005f;
    [SerializeField] private Ease spawnEase = Ease.OutBack;
    [Space]
    [SerializeField] private float spawnRadiusMin = 100f;
    [SerializeField] private float spawnRadiusMax = 200f;
    [Space]
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private Ease moveEase = Ease.InQuad;
    [Space]
    [SerializeField] private float spawnScale = 1.5f;
    [SerializeField] private float midScale = 0.5f;
    [SerializeField] private float endScale = 1f;

    [InjectService] private ICurrencyService _currencyService;

    private void Start()
    {
        DIContainer.InjectTo(this);

        animPrefab.SetActive(false);
        _currencyService.EventCurrencyAdded += OnCurrencyAdded;
    }

    private void OnDisable()
    {
        _currencyService.EventCurrencyAdded -= OnCurrencyAdded;
    }

    private void OnCurrencyAdded(CurrencyOperationData data)
    {
        if (data.Currency == currencyCounter.CurrencyType && data.UsePosition)
        {
            SpawnAnim(data.WorldPosition, data.Amount);
        }
    }

    private void SpawnAnim(Vector3 worldPosition, int amount)
    {
        int currentValue = currencyCounter.GetCurrentValue();

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);

        GameObject animInstance = Instantiate(animPrefab, transform);
        animInstance.SetActive(true);
        animInstance.transform.position = screenPos;

        TMP_Text text = animInstance.GetComponentInChildren<TMP_Text>();
        if (text != null)
        {
            text.text = $"{leftModificator}{amount}{rightModificator}";
        }
        
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector3 initialDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Random.Range(spawnRadiusMin, spawnRadiusMax);
        float duration = initialDirection.magnitude * spawnDuration;

        Vector3 start = screenPos;
        Vector3 mid = screenPos + initialDirection;
        Vector3 end = targetPoint.position;

        animInstance.transform.localScale = Vector3.zero;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(animInstance.transform.DOMove(animInstance.transform.position + initialDirection, duration).SetEase(spawnEase));
        sequence.Join(animInstance.transform.DOScale(spawnScale, duration).SetEase(spawnEase));

        sequence.Append(DOTween.To(() => 0f, x => 
        {
            animInstance.transform.localScale = EvaluateQuadraticBezier(Vector3.one * spawnScale, Vector3.one * midScale, Vector3.one * endScale, x);
            animInstance.transform.position = EvaluateQuadraticBezier(mid, start, end, x);
        }, 1f, moveDuration)
            .SetEase(moveEase));

        sequence.OnComplete(() => 
        {
            currencyCounter.SetValue(currentValue);
            currencyCounter.PlayUpdateAnim();
            Destroy(animInstance);
        });
    }

    private Vector3 EvaluateQuadraticBezier(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(ab, bc, t);
    }
}
