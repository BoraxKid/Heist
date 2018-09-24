using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable] public class FloatUnityEvent : UnityEvent<float> {}

public class FloatGameEventListener : MonoBehaviour
{
    [SerializeField] private FloatGameEvent _event;
    [SerializeField] private FloatUnityEvent _response;

    private void OnEnable()
    {
        this._event.RegisterListener(this);
    }

    private void OnDisable()
    {
        this._event.UnregisterListener(this);
    }

    public void OnEventRaised(float value)
    {
        this._response.Invoke(value);
    }
}
