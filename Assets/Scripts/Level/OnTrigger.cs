using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent _event;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider == GameConstants.playerVariable.collider)
        {
            Debug.Log("[" + this.name + "] -> OnTriggerEnter: " + collider.name);
            this._event.Invoke();
        }
    }
}
