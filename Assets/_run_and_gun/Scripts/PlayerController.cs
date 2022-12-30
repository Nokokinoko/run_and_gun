using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody m_Rigidbody;
    private Collider m_Collider;
    private bool m_IsMovable = false;
    private bool m_HasTarget = false;
    
    private Vector2 m_PointRef = Vector2.zero;
    private Vector2 m_NormalizedDirection = Vector2.zero;
    private Vector3 m_PrevVelocity = Vector3.zero;

    [SerializeField] private float m_MoveRatio;
    [SerializeField] private Transform m_Tower;
    [SerializeField] private List<Transform> m_ListWheels;

    [Space]
    [SerializeField] private Transform m_ParentBullet;
    [SerializeField] private GameObject m_PrefabBullet;

    private EnemyManager m_MgrEnemy;
    public EnemyManager MgrEnemy { set { m_MgrEnemy = value; } }

    private const float TOUCH_RADIUS = 50.0f;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        
        m_Collider = GetComponent<Collider>();
        m_Collider.OnTriggerEnterAsObservable()
            .Where(_collider => _collider.CompareTag(GameDefinitions.TAG_ENEMY))
            .Subscribe(_ => OnDamage())
            .AddTo(this);

        this.UpdateAsObservable()
            .Subscribe(_ => {
                if (m_IsMovable)
                {
                    ProcessTouch();
                    
                    // 砲台を敵に向ける
                    EnemyController _ctrl = m_MgrEnemy.Nearest();
                    if (_ctrl != null && _ctrl.Distance < GameDefinitions.DISTANCE)
                    {
                        TargetEnemy(_ctrl);
                        m_HasTarget = true;
                    }
                    else
                    {
                        m_Tower.localEulerAngles = Vector3.zero;
                        m_HasTarget = false;
                    }
                }

                if (m_NormalizedDirection == Vector2.zero)
                {
                    m_Rigidbody.velocity = Vector3.zero;
                    m_Rigidbody.angularVelocity = Vector3.zero;
                }
                else
                {
                    Vector3 _Velocity = new Vector3(
                        m_NormalizedDirection.x,
                        0.0f,
                        m_NormalizedDirection.y
                    );
                    Vector3 _Lerp = Vector3.Lerp(m_PrevVelocity, _Velocity, Time.deltaTime);
                    m_Rigidbody.velocity = _Lerp * m_MoveRatio;
                    m_PrevVelocity = _Velocity;

                    // 自機の回転
                    m_Rigidbody.rotation = Quaternion.Slerp(
                        m_Rigidbody.rotation,
                        Quaternion.LookRotation(_Lerp),
                        0.8f
                    );

                    // タイヤの回転
                    m_ListWheels.ForEach(_wheel => _wheel.Rotate(new Vector3(180.0f, 0.0f, 0.0f) * Time.deltaTime));
                }
            })
            .AddTo(this);

        Observable.Interval(TimeSpan.FromSeconds(GameDefinitions.BULLET_INTERVAL))
            .Where(_ => m_HasTarget)
            .Subscribe(_ => Fire())
            .AddTo(this);
    }

    private void ProcessTouch()
    {
        switch (InputManager.GetTouch())
        {
            case ENUM_TOUCH.TOUCH_BEGAN:
                m_PointRef = InputManager.GetPosition();
                m_PrevVelocity = Vector3.zero;
                break;
            case ENUM_TOUCH.TOUCH_MOVED:
                Vector2 _touch = InputManager.GetPosition();
                if (TOUCH_RADIUS < Vector2.Distance(_touch, m_PointRef))
                {
                    m_NormalizedDirection = (_touch - m_PointRef).normalized;
                    // 基準点が向かうポイントを決定
                    Vector2 _to = _touch - (m_NormalizedDirection * TOUCH_RADIUS);
                    m_PointRef = Vector2.Lerp(m_PointRef, _to, Time.deltaTime * 2.0f);
                }
                else
                {
                    m_NormalizedDirection = Vector2.zero;
                }
                break;
            case ENUM_TOUCH.TOUCH_ENDED:
                m_PointRef = Vector2.zero;
                m_NormalizedDirection = Vector2.zero;
                break;
        }
    }

    private void TargetEnemy(EnemyController ctrl)
    {
        Vector3 _diff = ctrl.Position - transform.position;
        float _rotateY = Mathf.Atan2(_diff.x, _diff.z) * Mathf.Rad2Deg;
        float _bodyY = transform.eulerAngles.y;
        m_Tower.localEulerAngles = new Vector3(0.0f, _rotateY + _bodyY * -1, 0.0f);
    }

    private void Fire()
    {
        GameObject _prefab = Instantiate(m_PrefabBullet, m_ParentBullet, true);
        Bullet _bullet = _prefab.GetComponent<Bullet>();
        Vector3 _position = transform.position;
        _position.y = 3.0f;
        _bullet.transform.SetPositionAndRotation(_position, m_Tower.rotation);
        _bullet.Fire();
    }

    public void GameStart()
    {
        m_IsMovable = true;
    }

    private void OnDamage()
    {
        m_IsMovable = false;
        m_NormalizedDirection = Vector2.zero;

        m_Collider.enabled = false;
        
        GameEventManager.Notify(GameEvent.ResultFail);
    }
}
