using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ResultFailUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_TextStage;
    [SerializeField] private TextMeshProUGUI m_TextMoney;
    [SerializeField] private Button m_ButtonNext;
    
    public IObservable<RectTransform> RxNext =>
        m_ButtonNext
            .OnClickAsObservable()
            .Select(_ => m_ButtonNext.GetComponent<RectTransform>());

    public void Activate()
    {
        m_TextStage.text = "Stage " + SaveData.Stage.ToString() + " Fail...";
        m_TextMoney.text = "+ 100";
        
        gameObject.SetActive(true);
    }
}
