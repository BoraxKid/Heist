using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Game event", fileName = "New game event")]
public class GameEvent : ScriptableObject
{
    private List<GameEventListener> _listeners = new List<GameEventListener>();

    public void RegisterListener(GameEventListener listener)
    {
        if (!this._listeners.Contains(listener))
            this._listeners.Add(listener);
        else
            Debug.LogWarning("Attempting to register listener of " + listener.name + " but it is already present");
    }

    public void UnregisterListener(GameEventListener listener)
    {
        if (this._listeners.Contains(listener))
            this._listeners.Remove(listener);
        else
            Debug.LogWarning("Attempting to unregister listener of " + listener.name + " but it is not present");
    }

    public void Raise()
    {
        for (int i = this._listeners.Count - 1; i >= 0; --i)
            this._listeners[i].OnEventRaised();
    }
}
