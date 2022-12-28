using System;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

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
    private Collider m_Collider;
    private bool m_IsGaming = false;
    
    private PlayerController m_Target;
    public PlayerController Target { set { m_Target = value; } }

    private const string TAG_BULLET = "Bullet";

    private void OnValidate()
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
        m_Collider = GetComponent<Collider>();
        
        this.UpdateAsObservable()
            .Where(_ => m_IsGaming && m_Target)
            .Subscribe(_ => {
                Vector3 _direction = m_Target.transform.position - m_Transform.position;
                _direction.y = 0.0f;
                Quaternion _rotation = Quaternion.LookRotation(_direction, Vector3.up);
                m_Transform.rotation = _rotation;
                m_Transform.position += m_Transform.forward * 0.04f;
            })
            .AddTo(this);

        m_Collider.OnTriggerEnterAsObservable()
            .Where(_collider => _collider.CompareTag(TAG_BULLET))
            .Subscribe(_ => OnHit())
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
    }

    private void OnHit()
    {
        
    }
}
