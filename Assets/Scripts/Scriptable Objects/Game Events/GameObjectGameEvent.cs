using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/GameObject Game event", fileName = "New GameObject game event")]
public class GameObjectGameEvent : ScriptableObject
{
    private List<GameObjectGameEventListener> _gameObjectListeners = new List<GameObjectGameEventListener>();
    private List<GameEventListener> _listeners = new List<GameEventListener>();

    public void RegisterListener(GameObjectGameEventListener listener)
    {
        if (!this._gameObjectListeners.Contains(listener))
            this._gameObjectListeners.Add(listener);
        else
            Debug.LogWarning("Attempting to register listener of " + listener.name + " but it is already present");
    }

    public void UnregisterListener(GameObjectGameEventListener listener)
    {
        if (this._gameObjectListeners.Contains(listener))
            this._gameObjectListeners.Remove(listener);
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

    public void Raise(GameObject value)
    {
        for (int i = this._gameObjectListeners.Count - 1; i >= 0; --i)
            this._gameObjectListeners[i].OnEventRaised(value);
        for (int i = this._listeners.Count -1; i >= 0; --i)
            this._listeners[i].OnEventRaised();
    }
}
