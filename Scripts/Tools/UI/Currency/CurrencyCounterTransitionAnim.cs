using DG.Tweening;
using Larje.Core;
using Larje.Core.Services;
using ProjectConstants;
using TMPro;
using UnityEngine;

public class CurrencyCounterTransitionAnim : MonoBehaviour
{
    [SerializeField] private Transform targetPoint;
    [SerializeField] private GameObject animPrefab;
    [Space]
    [SerializeField] private string leftModificator;
    [SerializeField] private string rightModificator;

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
        if (data.Currency == CurrencyType.Coins && data.UsePosition)
        {
            SpawnAnim(data.WorldPosition, data.Amount);
        }
    }

    private void SpawnAnim(Vector3 worldPosition, int amount)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);

        GameObject animInstance = Instantiate(animPrefab, transform);
        animInstance.SetActive(true);
        animInstance.transform.position = screenPos;

        TMP_Text text = animInstance.GetComponentInChildren<TMP_Text>();
        if (text != null)
        {
            text.text = $"{leftModificator}{amount}{rightModificator}";
        }

        animInstance.transform.DOMove(targetPoint.position, 1f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => Destroy(animInstance));
    }
}
