using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PrefabPool : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _count = 10;

    private Dictionary<GameObject, bool> _prefabs = new Dictionary<GameObject, bool>();

    private void Awake()
    {
        for (int i = 0; i < this._count; ++i)
            this._prefabs.Add(Instantiate(this._prefab, this.gameObject.transform), false);
    }

    public GameObject Request()
    {
        GameObject result = null;
        // Search for a free gameObject
        foreach (KeyValuePair<GameObject, bool> pair in this._prefabs)
        {
            if (!pair.Value)
            {
                // Found one!
                // Set it busy (true), keep it in result and exit the loop
                this._prefabs[pair.Key] = true;
                result = pair.Key;
                break;
            }
        }
        // If we couldn't find a free GameObject, Instantiate a new one, add it to the _prefabs list and keep it
        if (result == null)
        {
            result = Instantiate(this._prefab, this.gameObject.transform);
            this._prefabs.Add(result, true);
        }
        return (result);
    }

    public void Release(GameObject gameObject)
    {
        this._prefabs[gameObject] = false;
    }
}
