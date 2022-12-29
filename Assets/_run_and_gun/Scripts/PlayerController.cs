using System;
using System.Linq;
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
    
    private Vector2 m_PointRef = Vector2.zero;
    private Vector2 m_NormalizedDirection = Vector2.zero;
    private Vector3 m_PrevVelocity = Vector3.zero;

    [SerializeField] private float m_MoveRatio;
    [SerializeField] private Animator m_Animator;

    private const float RADIUS = 50.0f;

    private const string ANIME_IDLE = "Idle";
    private const string ANIME_WALK = "Walk";
    private const string TAG_ENEMY = "Enemy";
    
    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        
        m_Collider = GetComponent<Collider>();
        m_Collider.OnTriggerEnterAsObservable()
            .Where(_collider => _collider.CompareTag(TAG_ENEMY))
            .Subscribe(_ => OnDamage())
            .AddTo(this);

        foreach (var _child in GetComponentsInChildren<Collider>())
        {
            if (m_Collider != _child)
            {
                _child.enabled = false;
            }
        }

        this.UpdateAsObservable()
            .Subscribe(_ => {
                if (m_IsMovable)
                {
                    switch (InputManager.GetTouch())
                    {
                        case ENUM_TOUCH.TOUCH_BEGAN:
                            m_PointRef = InputManager.GetPosition();
                            m_PrevVelocity = Vector3.zero;
                            break;
                        case ENUM_TOUCH.TOUCH_MOVED:
                            Vector2 _touch = InputManager.GetPosition();
                            if (RADIUS < Vector2.Distance(_touch, m_PointRef))
                            {
                                m_NormalizedDirection = (_touch - m_PointRef).normalized;
                                // 基準点が向かうポイントを決定
                                Vector2 _to = _touch - (m_NormalizedDirection * RADIUS);
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

                if (m_NormalizedDirection == Vector2.zero)
                {
                    m_Rigidbody.velocity = Vector3.zero;
                    m_Rigidbody.angularVelocity = Vector3.zero;
                    m_Animator.Play(ANIME_IDLE);
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

                    m_Animator.Play(ANIME_WALK);

                    m_Rigidbody.rotation = Quaternion.Slerp(
                        m_Rigidbody.rotation,
                        Quaternion.LookRotation(_Lerp),
                        0.8f
                    );
                }
            })
            .AddTo(this);
    }

    public void GameStart()
    {
        m_IsMovable = true;
    }

    private void OnDamage()
    {
        m_IsMovable = false;

        m_Collider.enabled = false;
        m_Animator.enabled = false;
        
        foreach (var _child in GetComponentsInChildren<Collider>())
        {
            if (m_Collider != _child)
            {
                _child.enabled = true;
                Rigidbody _rigidbody = _child.GetComponent<Rigidbody>();
                _rigidbody.isKinematic = false;
            }
        }
        
        GameEventManager.Notify(GameEvent.ResultFail);
    }
}
