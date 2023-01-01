using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class TopUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_TextStage;

    [SerializeField] private Button m_ButtonStart;

    private void Awake()
    {
        m_TextStage.text = "Stage " + SaveData.Stage.ToString();

        m_ButtonStart.OnClickAsObservable()
            .First()
            .Subscribe(_ => GameEventManager.Notify(GameEvent.GameStart))
            .AddTo(this);
    }
}
