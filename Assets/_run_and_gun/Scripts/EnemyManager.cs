using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private readonly List<EnemyController> m_ListEnemys = new List<EnemyController>();

    private PlayerController m_CtrlPlayer = null;
    public PlayerController CtrlPlayer { set { m_CtrlPlayer = value; } }
    
    public void GraspEnemy()
    {
        int _loop = Mathf.FloorToInt((float)(SaveData.Stage - 1) / GameDefinitions.NUM_STAGE);

        foreach (var _child in GetComponentsInChildren<EnemyController>())
        {
            _child.Plus(_loop);
            _child.Target = m_CtrlPlayer.transform;
            m_ListEnemys.Add(_child);
        }
    }

    public void GameStart()
    {
        m_ListEnemys.ForEach(_ctrl => _ctrl.GameStart());
        
        this.ObserveEveryValueChanged(_ => NumDestroyEnemy)
            .Where(_num => NumEnemy <= _num)
            .Subscribe(_ => GameEventManager.Notify(GameEvent.ResultClear))
            .AddTo(this);
    }

    public void GameEnd()
    {
        m_ListEnemys.ForEach(_ctrl => _ctrl.GameEnd());
    }

    public EnemyController Nearest()
    {
        return m_ListEnemys.Where(_ctrl => _ctrl.IsAlive).OrderBy(_ctrl => _ctrl.Distance).FirstOrDefault();
    }

    public int NumEnemy => m_ListEnemys.Count;
    public int NumDestroyEnemy => m_ListEnemys.Count(_ctrl => !_ctrl.IsAlive);
    public int Coin => NumDestroyEnemy * GameDefinitions.MONEY_BY_ENEMY;
}
