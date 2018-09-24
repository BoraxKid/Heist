using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTarget : MonoBehaviour
{
    [SerializeField] private Transform _head;
    [SerializeField] private GameObject _target;
    private HeadLookController _headLookController;

    private void Awake()
    {
        if (this._head == null)
            this._head = this.transform;
        this._headLookController = this.GetComponent<HeadLookController>();
    }

    public void SetTarget(GameObject gameObject)
    {
        this._target = gameObject;
    }

    private void LateUpdate()
    {
        if (GameConstants.paused)
            return;
        if (this._target != null)
        {
            if (this._headLookController != null)
                this._headLookController.target = Helper.GetTop(this._target);
            else
                this._head.LookAt(Helper.GetTop(this._target));
        }
    }
}
