using Cysharp.Threading.Tasks;
using UnityEngine;

public class StageLoader : MonoBehaviour
{
    private bool m_IsLoaded = false;
    public bool IsLoaded => m_IsLoaded;
    
    private EnemyManager m_MgrEnemy;
    public EnemyManager MgrEnemy { set { m_MgrEnemy = value; } }

    private async UniTask Start()
    {
        await UniTask.WaitUntil(() => m_MgrEnemy != null);
        
        Load();
        m_IsLoaded = true;
    }

    private void Load()
    {
        int _num = SaveData.Stage % GameDefinitions.NUM_STAGE;
        if (_num == 0)
        {
            _num = GameDefinitions.NUM_STAGE;
        }
        
        var assetName = $"Stage/Stage_{_num:00}";
        var prefab = AssetLoader.Load<GameObject>(assetName);
        var obj = Instantiate(prefab, transform);
        obj.transform.position = Vector3.zero;
    }
}
