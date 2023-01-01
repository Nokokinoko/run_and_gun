using System;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    private static MoneyManager m_Instance = null;
    public static MoneyManager Instance => m_Instance;

    [SerializeField] private MoneyGetEffectUI m_Effect;
    [SerializeField][Range(0.1f, 1.0f)] private float m_RatioEffect;

    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        else if (Instance != this)
        {
            // 複数アタッチされているので破棄
            Destroy (this);
        }
    }

    public void GetMoney(int get)
    {
        SaveData.Money += get;
    }

    public void GetMoneyByEffect(RectTransform from, int get, Action onComplete)
    {
        m_Effect.ShowEffect(from.position, Mathf.CeilToInt(get * m_RatioEffect), () => {
            GetMoney(get);
            onComplete?.Invoke();
        });
    }

    public bool UseMoney(int use)
    {
        if (SaveData.Money - use < 0)
        {
            return false;
        }

        SaveData.Money -= use;
        return true;
    }
}
