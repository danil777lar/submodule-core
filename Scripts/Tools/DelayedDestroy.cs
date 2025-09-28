using DG.Tweening;
using UnityEngine;

public class DelayedDestroy : MonoBehaviour
{
    [SerializeField] private float delay = 3f;
    [SerializeField] private float animDuration = 2f;

    private void Start()
    {
        DOVirtual.DelayedCall(delay, () =>
        {
            transform.DOScale(Vector3.zero * 0.001f, animDuration)
                .OnComplete(() => Destroy(gameObject));
        });
    }
}
