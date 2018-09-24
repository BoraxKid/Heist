using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Variables/Detection", fileName = "New detection variable")]
public class DetectionVariable : ScriptableObject
{
    [SerializeField] private GameObjectGameEvent _onRegisterEnemy;
    [SerializeField] private GameObjectGameEvent _onUnregisterEnemy;

    public List<GameObject> enemies;

    public void Clear()
    {
        this.enemies.Clear();
    }

    public void RegisterEnemy(GameObject gameObject)
    {
        if (!this.enemies.Contains(gameObject))
        {
            this.enemies.Add(gameObject);
            this._onRegisterEnemy.Raise(gameObject);
        }
    }

    public void UnregisterEnemy(GameObject gameObject)
    {
        if (this.enemies.Contains(gameObject))
        {
            this.enemies.Remove(gameObject);
            this._onUnregisterEnemy.Raise(gameObject);
        }
    }
}
