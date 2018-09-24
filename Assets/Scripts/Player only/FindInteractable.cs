using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindInteractable : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private FloatReference _radius;
    [SerializeField] private FloatReference _updateTime;

    public Interactable Nearest
    {
        get;
        private set;
    }

    private void OnEnable()
    {
        this.InvokeRepeating("Find", 0.0f, this._updateTime.Value);
    }

    private void OnDisable()
    {
        this.CancelInvoke("Find");
    }

    private void Find()
    {
        GameObject nearest = Helper.FindNearestInLayer(Helper.GetCenter(GameConstants.playerVariable.gameObject), this._radius.Value, this._layerMask, false);
        if (nearest == null)
        {
            if (this.Nearest != null)
            {
                this.Nearest.enabled = false;
                this.Nearest = null;
            }
            return;
        }
        if (this.Nearest != null)
        {
            if (this.Nearest.gameObject != nearest)
                this.Nearest.enabled = false;
        }
        this.Nearest = nearest.GetComponent<Interactable>();
        if (this.Nearest && !this.Nearest.Used && !GameConstants.paused)
            this.Nearest.enabled = true;
    }
}
