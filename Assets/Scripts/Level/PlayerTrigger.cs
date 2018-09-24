using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent _onPlayerTriggerEnter;
    [SerializeField] private UnityEvent _onPlayerTriggerExit;

    private int _count = 0;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider == GameConstants.playerVariable.collider)
        {
            ++this._count;
            if (this._count == 1)
                this._onPlayerTriggerEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider == GameConstants.playerVariable.collider)
        {
            --this._count;
            if (this._count == 0)
                this._onPlayerTriggerExit.Invoke();
        }
    }
}
