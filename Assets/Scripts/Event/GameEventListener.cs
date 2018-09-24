using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    [SerializeField] private GameEvent _event;
    [SerializeField] private UnityEvent _response;

    private void OnEnable()
    {
        this._event.RegisterListener(this);
    }

    private void OnDisable()
    {
        this._event.UnregisterListener(this);
    }

    public void OnEventRaised()
    {
        this._response.Invoke();
    }
}
