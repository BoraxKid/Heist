using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnAwake : MonoBehaviour
{
    [SerializeField] private UnityEvent _onAwakeEvent;

    private void Awake()
    {
        this._onAwakeEvent.Invoke();
    }
}
