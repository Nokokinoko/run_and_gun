using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
public class EnemyController : MonoBehaviour
{
    [Serializable]
    public enum ENUM_ENEMY_LEVEL
    {
        ENEMY_LEVEL_1 = 1,
        ENEMY_LEVEL_2 = 2,
        ENEMY_LEVEL_3 = 3,
    }

    [SerializeField] private Animator m_Animator;
    [SerializeField] private SkinnedMeshRenderer m_Renderer;
    [SerializeField] private ENUM_ENEMY_LEVEL m_Level;
    [SerializeField] private List<Material> m_ListMaterials;

    private Transform m_Transform;
    public Vector3 Position => m_Transform.position;
    private NavMeshAgent m_NavAgent;
    private Collider m_Collider;
    private bool m_IsGaming = false;
    
    private Transform m_Target;
    public Transform Target { set { m_Target = value; } }

    private bool m_IsAlive = true;
    public bool IsAlive => m_IsAlive;

    private float m_Distance;
    public float Distance => m_Distance;

    private const string KEY_ANIMATOR_IS_ALIVE = "IsAlive";
    private const string NAME_ANIMATION_DEATH = "Death";

    private void ChangeMaterial()
    {
        int _key = (int)m_Level - 1;
        if (_key < 0 || m_ListMaterials.Count < _key)
        {
            // do not process
            return;
        }

        Material _mat = m_ListMaterials[_key];
        Material[] _renderer = m_Renderer.materials;
        _renderer[GameDefinitions.KEY_MATERIAL_BODY] = _mat;
        m_Renderer.materials = _renderer;
    }

    private void Awake()
    {
        ChangeMaterial();
        
        m_Transform = transform;
        m_NavAgent = GetComponent<NavMeshAgent>();
        m_NavAgent.speed = 0.0f;
        m_Collider = GetComponent<Collider>();
        
        this.UpdateAsObservable()
            .Where(_ => m_IsGaming && m_Target)
            .Subscribe(_ => {
                Vector3 _targetPosition = m_Target.position;
                _targetPosition.y = Position.y;
                
                // 二点間の距離
                m_Distance = Vector3.Distance(_targetPosition, Position);
                m_NavAgent.speed = (m_Distance < GameDefinitions.DISTANCE) ? GameDefinitions.ENEMY_SPEED : 0.0f;
                
                Vector3 _direction = _targetPosition - Position;
                _direction.y = 0.0f;
                Quaternion _rotation = Quaternion.LookRotation(_direction, Vector3.up);
                m_Transform.rotation = _rotation;

                m_NavAgent.SetDestination(_targetPosition);
            })
            .AddTo(this);

        m_Collider.OnTriggerEnterAsObservable()
            .Where(_collider => _collider.CompareTag(GameDefinitions.TAG_AXE))
            .Subscribe(_collider => OnHit(_collider).Forget())
            .AddTo(this);
    }

    public void GameStart()
    {
        m_IsGaming = true;
    }

    public void GameEnd()
    {
        m_IsGaming = false;
        m_NavAgent.enabled = false;
    }

    private async UniTask OnHit(Collider collider)
    {
        Destroy(collider.gameObject);

        if (m_Level == ENUM_ENEMY_LEVEL.ENEMY_LEVEL_1)
        {
            m_IsAlive = false;
            m_Animator.SetBool(KEY_ANIMATOR_IS_ALIVE, false);
            
            await UniTask.WaitUntil(() => m_Animator.GetCurrentAnimatorStateInfo(0).IsName(NAME_ANIMATION_DEATH));

            await UniTask.WaitUntil(() => 1.0f <= m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            
            gameObject.SetActive(false);
        }
        else
        {
            m_Level = (ENUM_ENEMY_LEVEL)((int)m_Level - 1);
            ChangeMaterial();
        }
    }
}
