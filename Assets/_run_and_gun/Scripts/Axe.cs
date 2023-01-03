using System;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class Axe : MonoBehaviour
{
    [SerializeField] private float m_Speed;
    [SerializeField] private Transform m_Body;
    
    public void Throw()
    {
        transform.position += transform.forward;
        
        this.UpdateAsObservable()
            .Subscribe(_ => transform.position += transform.forward * m_Speed * Time.deltaTime)
            .AddTo(this);

        m_Body.DOLocalRotate(new Vector3(0.0f, 360.0f, 0.0f), 0.2f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1);

        Observable.Timer(TimeSpan.FromSeconds(GameDefinitions.AXE_LIFETIME))
            .Subscribe(_ => Destroy(gameObject))
            .AddTo(this);
    }
}
