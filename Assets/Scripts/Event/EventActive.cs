using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventActive : MonoBehaviour
{
    [SerializeField] private UnityEvent _onEnabledEvent;
    [SerializeField] private UnityEvent _onDisabledEvent;

    private void OnEnable()
    {
        this._onEnabledEvent.Invoke();
    }

    private void OnDisable()
    {
        this._onDisabledEvent.Invoke();
    }
}
