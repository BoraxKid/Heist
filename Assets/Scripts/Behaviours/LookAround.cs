using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HeadLookController))]
public class LookAround : MonoBehaviour
{
    [SerializeField] private FloatReference _turnHeadTime;
    [SerializeField] private FloatReference _alternateTurnHeadTime;
    [SerializeField] private Transform _head;

    private HeadLookController _headLookController;
    private LookTarget _lookTarget;
    private float _elapsedTime;
    private bool _left;
    private bool _useAlternateTime = false;

    private void Awake()
    {
        this._headLookController = this.GetComponent<HeadLookController>();
        this._lookTarget = this.GetComponent<LookTarget>();
    }

    private void OnEnable()
    {
        this._elapsedTime = 0.0f;
    }

    public void UseAlternateTime(bool value)
    {
        this._useAlternateTime = value;
    }

    private void LateUpdate()
    {
        if (GameConstants.paused)
            return;
        this._elapsedTime += Time.deltaTime;
        float time = (this._useAlternateTime) ? this._alternateTurnHeadTime.Value : this._turnHeadTime.Value;
        if (this._elapsedTime >= time)
        {
            this._elapsedTime -= time;
            if (this._lookTarget == null || (this._lookTarget != null && !this._lookTarget.enabled))
                this.TurnHead();
        }
    }

    private void TurnHead()
    {
        this._left = !this._left;

        if (this._left)
            this._headLookController.target = this._head.position - this.transform.right;
        else
            this._headLookController.target = this._head.position + this.transform.right;
    }
}
