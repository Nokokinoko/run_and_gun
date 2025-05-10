using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TopUI m_TopUI;
    [SerializeField] private GameObject m_Tutorial;
    [SerializeField] private IngameUI m_IngameUI;
    [SerializeField] private ResultClearUI m_ResultClearUI;
    [SerializeField] private ResultFailUI m_ResultFailUI;

    [Space]
    [SerializeField] private MoneyUI m_MoneyUI;

    private EnemyManager m_MgrEnemy;
    public EnemyManager MgrEnemy { set { m_MgrEnemy = value; } }

    private const float DELAY_RELOAD = 1.0f;
    
    private void Awake()
    {
        m_ResultClearUI.RxNext
            .First()
            .Subscribe(_transform => {
                MoneyManager.Instance.GetMoneyByEffect(
                    _transform,
                    m_MgrEnemy.Coin,
                    () => Observable.Timer(TimeSpan.FromSeconds(DELAY_RELOAD))
                        .Subscribe(_ => {
                            SaveData.Stage++;
                            ReloadManager.Instance.Reload();
                        })
                        .AddTo(this)
                );
            })
            .AddTo(this);
            
        m_ResultFailUI.RxNext
            .First()
            .Subscribe(_transform => {
                MoneyManager.Instance.GetMoneyByEffect(
                    _transform,
                    m_MgrEnemy.Coin,
                    () => Observable.Timer(TimeSpan.FromSeconds(DELAY_RELOAD))
                        .Subscribe(_ => ReloadManager.Instance.Reload())
                        .AddTo(this)
                );
            })
            .AddTo(this);
    }

    private async UniTask Start()
    {
        await UniTask.WaitUntil(() => m_MgrEnemy != null);
        
        this.ObserveEveryValueChanged(_ => m_MgrEnemy.NumDestroyEnemy)
            .Subscribe(_num => m_IngameUI.SetTextNumDestroy(_num))
            .AddTo(this);
    }

    public void EnableTopUI()
    {
        m_TopUI.gameObject.SetActive(true);
        m_Tutorial.SetActive(true);
        m_IngameUI.gameObject.SetActive(false);
        m_ResultClearUI.gameObject.SetActive(false);
        m_ResultFailUI.gameObject.SetActive(false);
        m_MoneyUI.gameObject.SetActive(true);
    }

    public void EnableIngameUI()
    {
        m_IngameUI.SetTextNum(m_MgrEnemy.NumEnemy);

        m_TopUI.gameObject.SetActive(false);
        m_Tutorial.SetActive(false);
        m_IngameUI.gameObject.SetActive(true);
        m_ResultClearUI.gameObject.SetActive(false);
        m_ResultFailUI.gameObject.SetActive(false);
        m_MoneyUI.gameObject.SetActive(false);
    }

    public void EnableResultClearUI()
    {
        m_TopUI.gameObject.SetActive(false);
        m_Tutorial.SetActive(false);
        m_IngameUI.gameObject.SetActive(false);
        m_ResultClearUI.Activate(m_MgrEnemy.Coin);
        m_ResultFailUI.gameObject.SetActive(false);
        m_MoneyUI.gameObject.SetActive(true);
    }

    public void EnableResultFailUI()
    {
        m_TopUI.gameObject.SetActive(false);
        m_Tutorial.SetActive(false);
        m_IngameUI.gameObject.SetActive(false);
        m_ResultClearUI.gameObject.SetActive(false);
        m_ResultFailUI.Activate(m_MgrEnemy.Coin);
        m_MoneyUI.gameObject.SetActive(true);
    }
}
