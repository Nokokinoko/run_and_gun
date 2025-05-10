using System;
using Cysharp.Threading.Tasks;
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

    public void Activate(int coin)
    {
        m_TextStage.text = "Stage " + SaveData.Stage.ToString() + " Fail...";
        m_TextMoney.text = "+ " + coin.ToString();
        
        gameObject.SetActive(true);
        
        Interactable().Forget();
    }

    private async UniTask Interactable()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1.0f));

        m_ButtonNext.interactable = true;
    }
}
