using System;
using DG.Tweening;
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
    private Vector3 m_TargetDir = Vector3.one;
    
    private Vector2 m_PointRef = Vector2.zero;
    private Vector2 m_NormalizedDirection = Vector2.zero;
    private Vector3 m_PrevVelocity = Vector3.zero;

    [SerializeField] private float m_MoveRatio;
    [SerializeField] private Transform m_Body;
    [SerializeField] private Animator m_Animator;

    [Space]
    [SerializeField] private Transform m_ParentWeapon;
    [SerializeField] private GameObject m_PrefabWeapon;

    private EnemyManager m_MgrEnemy;
    public EnemyManager MgrEnemy { set { m_MgrEnemy = value; } }

    private const float TOUCH_RADIUS = 50.0f;

    private const string KEY_ANIMATOR_IS_MOVE = "IsMove";
    private const string NAME_ANIMATION_ATTACK = "Attack";
    private const string NAME_ANIMATION_DEATH = "Death";

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        
        m_Collider = GetComponent<Collider>();
        m_Collider.OnTriggerEnterAsObservable()
            .Where(_collider => _collider.CompareTag(GameDefinitions.TAG_ENEMY))
            .Subscribe(_ => OnDamage())
            .AddTo(this);
    }

    public void Preprocess()
    {
        m_Body.DOLocalRotate(Vector3.zero, 1.0f).SetEase(Ease.Linear);
    }

    public void GameStart()
    {
        m_IsMovable = true;
        
        this.UpdateAsObservable()
            .Subscribe(_ => {
                if (m_IsMovable)
                {
                    ProcessTouch();
                    
                    // 最寄りの敵を検知
                    EnemyController _ctrl = m_MgrEnemy.Nearest();
                    if (_ctrl != null && _ctrl.Distance < GameDefinitions.WEAPON_DISTANCE)
                    {
                        Vector3 _diff = _ctrl.Position - transform.position;
                        float _rotateY = Mathf.Atan2(_diff.x, _diff.z) * Mathf.Rad2Deg;
                        m_TargetDir = new Vector3(0.0f, _rotateY, 0.0f);
                    }
                    else
                    {
                        m_TargetDir = Vector3.one;
                    }
                }

                if (m_NormalizedDirection == Vector2.zero)
                {
                    m_Rigidbody.velocity = Vector3.zero;
                    m_Rigidbody.angularVelocity = Vector3.zero;
                    
                    m_Animator.SetBool(KEY_ANIMATOR_IS_MOVE, false);
                }
                else
                {
                    Vector3 _Velocity = new Vector3(
                        m_NormalizedDirection.x,
                        0.0f,
                        m_NormalizedDirection.y
                    );
                    Vector3 _Lerp = Vector3.Lerp(m_PrevVelocity, _Velocity, Time.deltaTime);
                    float _ratio = m_MoveRatio + GameDefinitions.LEVEL_BY_MOVE * SaveData.LevelMove;
                    m_Rigidbody.velocity = _Lerp * _ratio;
                    m_PrevVelocity = _Velocity;

                    // 自機の回転
                    m_Rigidbody.rotation = Quaternion.Slerp(
                        m_Rigidbody.rotation,
                        Quaternion.LookRotation(_Lerp),
                        0.8f
                    );
                    
                    m_Animator.SetBool(KEY_ANIMATOR_IS_MOVE, true);
                }
            })
            .AddTo(this);

        float _interval = GameDefinitions.WEAPON_INTERVAL - GameDefinitions.LEVEL_BY_THROW * SaveData.LevelThrow;
        Observable.Interval(TimeSpan.FromSeconds(_interval))
            .Where(_ => m_TargetDir != Vector3.one)
            .Subscribe(_ => ThrowWeapon())
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

    private void ThrowWeapon()
    {
        GameObject _prefab = Instantiate(m_PrefabWeapon, m_ParentWeapon, true);
        Weapon _weapon = _prefab.GetComponent<Weapon>();
        Vector3 _position = transform.position;
        _position.y = 3.0f;
        _weapon.transform.SetPositionAndRotation(_position, Quaternion.Euler(m_TargetDir));
        _weapon.Throw();
        
        // アニメーション後に自動でIdleへ遷移
        m_Animator.Play(NAME_ANIMATION_ATTACK);
    }

    public void LookAtCamera()
    {
        float _y = transform.eulerAngles.y;
        float _to = (_y < 0.0f) ? -180.0f - _y : 180.0f - _y;
        m_Body.DOLocalRotate(new Vector3(0.0f, _to, 0.0f), 1.0f).SetEase(Ease.Linear);
    }

    public void GameEnd()
    {
        m_IsMovable = false;
        m_TargetDir = Vector3.one;
        m_NormalizedDirection = Vector2.zero;
    }

    private void OnDamage()
    {
        m_Collider.enabled = false;
        m_Animator.Play(NAME_ANIMATION_DEATH);
        
        GameEventManager.Notify(GameEvent.ResultFail);
    }
}
