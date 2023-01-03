using Cysharp.Threading.Tasks;
using UnityEngine;

public class Stage : MonoBehaviour
{
    private EnemyManager m_MgrEnemy;
    public EnemyManager MgrEnemy { set { m_MgrEnemy = value; } }

    private async UniTask Start()
    {
        await UniTask.WaitUntil(() => m_MgrEnemy != null);
        
    }
}
