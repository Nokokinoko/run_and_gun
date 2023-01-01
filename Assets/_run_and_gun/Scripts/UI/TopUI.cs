using TMPro;
using UniRx;
using UnityEngine;

public class TopUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_TextStage;

    [SerializeField] private TouchableArea m_Start;

    private void Awake()
    {
        m_TextStage.text = "Stage " + SaveData.Stage.ToString();

        m_Start.OnClick
            .First()
            .Subscribe(_ => GameEventManager.Notify(GameEvent.GameStart))
            .AddTo(this);
    }
}
