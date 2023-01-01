using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyLabel;
    [SerializeField] private RectTransform icon;

    private const float AnimDuration = 0.3f;

    private Tween _tween;

    public RectTransform Icon => icon;

    private void Start()
    {
        UpdateLabel(SaveData.Money);
        
        this.ObserveEveryValueChanged(_ => SaveData.Money)
            .Pairwise()
            .Subscribe(pair => UpdateLabel(pair.Previous, pair.Current))
            .AddTo(this);
    }
    
    private void UpdateLabel(int money)
    {
        moneyLabel.text = money.ToString();
    }

    private void UpdateLabel(int prev, int cur)
    {
        _tween?.Kill(true);
        
        _tween = DOVirtual
            .Int(prev, cur, AnimDuration, money => UpdateLabel(money))
            .SetLink(gameObject);
    }
}
