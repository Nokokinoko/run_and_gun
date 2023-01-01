using System;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MoneyGetEffectUI : MonoBehaviour
{
    [SerializeField] private RectTransform to;
    [SerializeField] private Transform iconTemplate;
        
    [Space]
    [SerializeField] private float explodeRadius;
    [SerializeField] private float explodeDuration;
    [SerializeField] private float adsorbDuration;

    private TransformPool _moneyIconPool;

    private void Awake()
    {
        _moneyIconPool = new TransformPool(iconTemplate, transform);

        this
            .OnDestroyAsObservable()
            .Subscribe(_ => _moneyIconPool.Dispose())
            .AddTo(this);
    }

    public void ShowEffect(Vector3 fromPos, int num, Action onComplete)
    {
        bool adsorbing = false;

        for (var i = 0; i < num; ++i)
        {
            var icon = (RectTransform)_moneyIconPool.Rent();
            icon.position = fromPos;
            icon.SetAsFirstSibling();
            icon.GetComponent<Image>().SetNativeSize();

            var pos = icon.localPosition;
            var r = Mathf.Sqrt(Random.Range(0f, 1f)) * explodeRadius;
            var theta = Random.Range(-Mathf.PI, Mathf.PI);
            pos.x += r * Mathf.Cos(theta);
            pos.y += r * Mathf.Sin(theta);

            DOTween.Sequence()
                .Append(icon.DOLocalMove(pos, explodeDuration + Random.Range(-0.1f, 0.1f)).SetEase(Ease.OutExpo))
                .Append(icon.DOMove(to.position, adsorbDuration).SetEase(Ease.InQuad))
                .Join(icon.DOSizeDelta(to.sizeDelta, adsorbDuration).SetEase(Ease.Linear))
                .OnComplete(() => {
                    _moneyIconPool.Return(icon);
                    if (!adsorbing)
                    {
                        adsorbing = true;
                        onComplete?.Invoke();
                    }
                })
                .Play();
        }
    }
}
