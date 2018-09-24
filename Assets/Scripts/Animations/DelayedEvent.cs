using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayedEvent : MonoBehaviour
{
    [SerializeField] private FloatReference _delayTime;
    [SerializeField] private UnityEvent _delayedEvent;

    public void DelayEvent()
    {
        this.Invoke("InvokeDelayedEvent", this._delayTime.Value);
    }

    private void InvokeDelayedEvent()
    {
        this._delayedEvent.Invoke();
    }
}
