using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HeadLookController))]
public class LookAhead : MonoBehaviour
{
    [SerializeField] private Transform _head;
    private HeadLookController _headLookController;
    private LookTarget _lookTarget;

    private void Awake()
    {
        if (this._head == null)
            this._head = this.transform;
        this._headLookController = this.GetComponent<HeadLookController>();
        this._lookTarget = this.GetComponent<LookTarget>();
    }

    private void LateUpdate()
    {
        if (GameConstants.paused)
            return;
        if (this._lookTarget == null || (this._lookTarget != null && !this._lookTarget.enabled))
            this._headLookController.target = this._head.position + this.transform.forward;
    }
}
