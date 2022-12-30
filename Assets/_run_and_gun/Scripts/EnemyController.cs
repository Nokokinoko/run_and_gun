using System;
using System.Collections.Generic;
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

    [SerializeField] private MeshRenderer m_Renderer;
    [SerializeField] private ENUM_ENEMY_LEVEL m_Level;
    [SerializeField] private List<Material> m_ListMaterials;

    private Transform m_Transform;
    public Vector3 Position => m_Transform.position;
    private NavMeshAgent m_NavAgent;
    private Collider m_Collider;
    private bool m_IsGaming = false;
    
    private Transform m_Target;
    public Transform Target { set { m_Target = value; } }

    private float m_Distance;
    public float Distance => m_Distance;

    private void OnValidate()
    {
        ChangeMaterial();
    }

    private void ChangeMaterial()
    {
        int _key = (int)m_Level - 1;
        if (_key < 0 || m_ListMaterials.Count < _key)
        {
            // do not process
            return;
        }

        Material _mat = m_ListMaterials[_key];
        m_Renderer.material = _mat;
    }

    private void Awake()
    {
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
            .Where(_collider => _collider.CompareTag(GameDefinitions.TAG_BULLET))
            .Subscribe(_collider => OnHit(_collider))
            .AddTo(this);
    }

    private void Start()
    {
        DOTween.To(x => {
            Vector3 _rotation = m_Transform.rotation.eulerAngles;
            _rotation.z = x;
            m_Transform.rotation = Quaternion.Euler(_rotation);
        }, 0.0f, 360.0f, 2.0f)
            .SetEase(Ease.Linear)
            .SetLoops(-1);
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

    private void OnHit(Collider collider)
    {
        Destroy(collider.gameObject);

        if (m_Level == ENUM_ENEMY_LEVEL.ENEMY_LEVEL_1)
        {
            Destroy(gameObject);
        }
        else
        {
            m_Level = (ENUM_ENEMY_LEVEL)((int)m_Level - 1);
            ChangeMaterial();
        }
    }
}
