using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UpgradeButton : MonoBehaviour
{
    [Serializable]
    public enum ENUM_UPGRADE
    {
        UPGRADE_MOVE,
        UPGRADE_BULLET,
    }
    [SerializeField] private ENUM_UPGRADE m_Upgrade;

    [SerializeField] private TextMeshProUGUI m_TextLevel;
    [SerializeField] private TextMeshProUGUI m_TextMoney;

    private Button m_Button;

    private bool IsLevelMax => GameDefinitions.LEVEL_MAX <= GetLevel();
    
    private void Awake()
    {
        SetText();

        m_Button = GetComponent<Button>();
        m_Button.OnClickAsObservable()
            .Subscribe(_ => LevelUp())
            .AddTo(this);

        this.ObserveEveryValueChanged(_ => SaveData.Money)
            .Subscribe(_ => Check())
            .AddTo(this);
    }

    private void Start()
    {
        Check();
    }

    private void SetText()
    {
        m_TextLevel.text = "Lv." + (IsLevelMax ? "MAX" : GetLevel().ToString());
        m_TextMoney.text = IsLevelMax ? "-" : GetNeedMoney().ToString();
    }

    private int GetLevel()
    {
        switch (m_Upgrade)
        {
            case ENUM_UPGRADE.UPGRADE_MOVE:
                return SaveData.LevelMove;
            case ENUM_UPGRADE.UPGRADE_BULLET:
                return SaveData.LevelBullet;
        }
        return 1;
    }

    private int GetNeedMoney()
    {
        if (IsLevelMax)
        {
            return 0;
        }
        return GetLevel() * GameDefinitions.NEED_MONEY;
    }

    private void Check()
    {
        m_Button.interactable = (!IsLevelMax && GetNeedMoney() <= SaveData.Money);
    }

    private void LevelUp()
    {
        if (IsLevelMax)
        {
            // do not process
            return;
        }

        MoneyManager.Instance.UseMoney(GetNeedMoney());
        switch (m_Upgrade)
        {
            case ENUM_UPGRADE.UPGRADE_MOVE:
                SaveData.LevelMove++;
                break;
            case ENUM_UPGRADE.UPGRADE_BULLET:
                SaveData.LevelBullet++;
                break;
        }
        
        SetText();
    }
}
