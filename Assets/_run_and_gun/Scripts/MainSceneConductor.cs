using System;
using Cinemachine;
using UniRx;
using UnityEngine;

public class MainSceneConductor : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera m_VCBack;
    [SerializeField] private CinemachineVirtualCamera m_VCUp;

    [Space]
    [SerializeField] private UIManager m_MgrUI;
    [SerializeField] private Stage m_Stage;
    [SerializeField] private PlayerController m_CtrlPlayer;
    [SerializeField] private EnemyManager m_MgrEnemy;

    [Space]
    [SerializeField] private GameObject m_Effect;
    [SerializeField] private ParticleSystem m_Particle;
    
    private void Awake()
    {
        m_VCBack.enabled = true;
        m_VCUp.enabled = false;
        
        m_Effect.SetActive(false);
        m_Particle.Stop();
    }

    private void Start()
    {
        m_Stage.MgrEnemy = m_MgrEnemy;
        m_CtrlPlayer.MgrEnemy = m_MgrEnemy;
        m_MgrEnemy.CtrlPlayer = m_CtrlPlayer;
        
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
        
        m_MgrUI.EnableTopUI();
    }

    #region GAME EVENT
    private void GameStart()
    {
        m_VCUp.enabled = true;

        m_MgrUI.EnableIngameUI();
        Observable.Timer(TimeSpan.FromSeconds(GameDefinitions.TIME_CINEMACHINE_TRANSITION))
            .Subscribe(_ => {
                m_CtrlPlayer.GameStart();
                m_MgrEnemy.GameStart();
            })
            .AddTo(this);
    }

    private void ResultClear()
    {
        m_VCUp.enabled = false;
        
        m_MgrUI.EnableResultClearUI();
        m_CtrlPlayer.GameEnd();
        m_MgrEnemy.GameEnd();

        Transform _camera = Camera.main.transform;
        Vector3 _position = _camera.position + _camera.forward * 16.0f;
        _position.y = 8.0f;
        m_Effect.transform.position = _position;
        
        Observable.Timer(TimeSpan.FromSeconds(GameDefinitions.TIME_CINEMACHINE_TRANSITION))
            .Subscribe(_ => m_Effect.SetActive(true))
            .AddTo(this);
    }

    private void ResultFail()
    {
        m_VCUp.Follow = null;
        m_VCUp.LookAt = null;
        
        m_MgrUI.EnableResultFailUI();
        m_CtrlPlayer.GameEnd();
        m_MgrEnemy.GameEnd();
    }
    #endregion
}
