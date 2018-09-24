using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Float Game event", fileName = "New float game event")]
public class FloatGameEvent : ScriptableObject
{
    private List<FloatGameEventListener> _floatListeners = new List<FloatGameEventListener>();
    private List<GameEventListener> _listeners = new List<GameEventListener>();

    public void RegisterListener(FloatGameEventListener listener)
    {
        if (!this._floatListeners.Contains(listener))
            this._floatListeners.Add(listener);
        else
            Debug.LogWarning("Attempting to register listener of " + listener.name + " but it is already present");
    }

    public void UnregisterListener(FloatGameEventListener listener)
    {
        if (this._floatListeners.Contains(listener))
            this._floatListeners.Remove(listener);
        else
            Debug.LogWarning("Attempting to unregister listener of " + listener.name + " but it is not present");
    }

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

    public void Raise(float value)
    {
        for (int i = this._floatListeners.Count - 1; i >= 0; --i)
            this._floatListeners[i].OnEventRaised(value);
        for (int i = this._listeners.Count -1; i >= 0; --i)
            this._listeners[i].OnEventRaised();
    }
}
