using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float m_Speed;
    
    public void Fire()
    {
        transform.position += transform.forward;
        
        this.UpdateAsObservable()
            .Subscribe(_ => transform.position += transform.forward * m_Speed * Time.deltaTime)
            .AddTo(this);

        Observable.Timer(TimeSpan.FromSeconds(5.0f))
            .Subscribe(_ => Destroy(gameObject))
            .AddTo(this);
    }
}
