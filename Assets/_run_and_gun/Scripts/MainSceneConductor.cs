using System;
using Cinemachine;
using UniRx;
using UnityEngine;

public class MainSceneConductor : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera m_VCBack;
    [SerializeField] private CinemachineVirtualCamera m_VCUp;

    [SerializeField] private PlayerController m_CtrlPlayer;
    [SerializeField] private EnemyManager m_MgrEnemy;

    private void Awake()
    {
        m_VCBack.enabled = true;
        m_VCUp.enabled = false;
    }

    private void Start()
    {
        GameEventManager
            .OnReceivedAsObservable(GameEvent.GameStart)
            .Subscribe(_ => GameStart())
            .AddTo(this);
        
        GameEventManager
            .OnReceivedAsObservable(GameEvent.ResultClear)
            .Subscribe(_ => ResultClear())
            .AddTo(this);
        
        GameEventManager
            .OnReceivedAsObservable(GameEvent.ResultFail)
            .Subscribe(_ => ResultFail())
            .AddTo(this);
        
        Observable.Timer(TimeSpan.FromSeconds(1.0f))
            .Subscribe(_ => GameEventManager.Notify(GameEvent.GameStart));
    }

    #region GAME EVENT
    private void GameStart()
    {
        m_VCUp.enabled = true;
        
        m_CtrlPlayer.GameStart();
        m_MgrEnemy.GameStart();
    }

    private void ResultClear()
    {
        m_VCUp.enabled = false;
        
        m_MgrEnemy.GameEnd();
    }

    private void ResultFail()
    {
        m_VCUp.Follow = null;
        m_VCUp.LookAt = null;
        
        m_MgrEnemy.GameEnd();
    }
    #endregion
}
