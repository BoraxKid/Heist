using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable] public class GameObjectUnityEvent : UnityEvent<GameObject> {}

public class GameObjectGameEventListener : MonoBehaviour
{
    [SerializeField] private GameObjectGameEvent _event;
    [SerializeField] private GameObjectUnityEvent _response;

    private void OnEnable()
    {
        this._event.RegisterListener(this);
    }

    private void OnDisable()
    {
        this._event.UnregisterListener(this);
    }

    public void OnEventRaised(GameObject value)
    {
        this._response.Invoke(value);
    }
}
