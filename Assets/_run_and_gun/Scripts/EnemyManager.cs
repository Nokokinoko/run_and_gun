using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private readonly List<EnemyController> m_ListEnemys = new List<EnemyController>();

    private PlayerController m_CtrlPlayer = null;
    public PlayerController CtrlPlayer { set { m_CtrlPlayer = value; } }
    
    private async UniTask Start()
    {
        await UniTask.WaitUntil(() => m_CtrlPlayer != null);
        
        foreach (Transform _child in transform)
        {
            var _ctrl = _child.GetComponent<EnemyController>();
            _ctrl.Target = m_CtrlPlayer.transform;
            m_ListEnemys.Add(_ctrl);
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
        return m_ListEnemys.OrderBy(_ctrl => _ctrl.Distance).FirstOrDefault();
    }

    public int NumEnemy => m_ListEnemys.Count;
    public int NumDestroyEnemy => m_ListEnemys.Count(_ctrl => !_ctrl.IsAlive);
}
