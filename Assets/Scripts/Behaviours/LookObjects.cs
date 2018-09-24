using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HeadLookController))]
public class LookObjects : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private FloatReference _updateTime;
    [SerializeField] private FloatReference _radius;
    [SerializeField] private Transform _head;

    private HeadLookController _headLookController;
    private GameObject _closest;

    private void Awake()
    {
        this._headLookController = this.GetComponent<HeadLookController>();
    }

    private void OnEnable()
    {
        this.InvokeRepeating("CheckAround", 0.0f, this._updateTime.Value);
    }

    private void OnDisable()
    {
        this.CancelInvoke("CheckAround");
    }

    private void Update()
    {
        if (GameConstants.paused)
            return;
        if (this._closest != null)
        {
            this._headLookController.target = Helper.GetCenter(this._closest);
            Debug.DrawLine(this._head.position, Helper.GetCenter(this._closest), Color.blue);
        }
        else
            this._headLookController.target = this._head.position + this.transform.forward;
    }

    private void CheckAround()
    {
        if (GameConstants.paused)
            return;
        this._closest = Helper.FindNearestInLayer(this._head.position, this._radius.Value, this._layerMask.value);
        DebugExtension.DebugWireSphere(this._head.position, this._radius.Value, this._updateTime.Value);
    }
}
