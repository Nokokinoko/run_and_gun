using System;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ObservableEventTrigger))]
[RequireComponent(typeof(CanvasRenderer))]
public class TouchableArea : Graphic
{
    private ObservableEventTrigger m_Trigger = null;
    private ObservableEventTrigger Trigger
    {
        get
        {
            if (m_Trigger == null)
            {
                m_Trigger = GetComponent<ObservableEventTrigger>();
            }
            return m_Trigger;
        }
    }

    public IObservable<PointerEventData> OnDown => Trigger.OnPointerDownAsObservable();
    public IObservable<PointerEventData> OnUp => Trigger.OnPointerUpAsObservable();
    public IObservable<PointerEventData> OnClick => Trigger.OnPointerClickAsObservable();
}
