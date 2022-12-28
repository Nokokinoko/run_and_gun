using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private PlayerController m_CtrlPlayer;

    private readonly List<EnemyController> m_ListEnemys = new List<EnemyController>();

    private void Start()
    {
        foreach (Transform _child in transform)
        {
            m_ListEnemys.Add(_child.GetComponent<EnemyController>());
        }

        m_ListEnemys.ForEach(_ctrl => _ctrl.Target = m_CtrlPlayer);
    }

    public void GameStart()
    {
        foreach (var _ctrl in m_ListEnemys)
        {
            _ctrl.GameStart();
        }
    }

    public void GameEnd()
    {
        foreach (var _ctrl in m_ListEnemys)
        {
            _ctrl.GameEnd();
        }
    }
}
