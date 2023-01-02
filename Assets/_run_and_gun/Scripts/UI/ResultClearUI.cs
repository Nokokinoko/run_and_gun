using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ResultClearUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_TextStage;
    [SerializeField] private TextMeshProUGUI m_TextMoney;
    [SerializeField] private Button m_ButtonNext;
    
    public IObservable<RectTransform> RxNext =>
        m_ButtonNext
            .OnClickAsObservable()
            .Select(_ => m_ButtonNext.GetComponent<RectTransform>());

    public void Activate(int coin)
    {
        m_TextStage.text = "Stage " + SaveData.Stage.ToString() + " Clear!!";
        m_TextMoney.text = "+ " + coin.ToString();
        
        gameObject.SetActive(true);
    }
}
