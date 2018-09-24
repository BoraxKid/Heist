using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour
{
    [SerializeField] private bool _open = false;
    [SerializeField] private bool _locked = false;
    [SerializeField] private UnityEvent _onLocked;
    [SerializeField] private Color _lockedColor;
    [SerializeField] private Color _unlockedColor;
    [SerializeField] private Renderer[] _powerCables;

    private PlayAnimation _playAnimation;
    private Collider _collider;
    private Interactable _interactable;
    private Material _material;
    private List<Material> _materials = new List<Material>();

    private void Awake()
    {
        this._playAnimation = this.GetComponent<PlayAnimation>();
        this._collider = this.GetComponent<Collider>();
        this._interactable = this.GetComponent<Interactable>();
        this._material = this.GetComponent<Renderer>().material;
        foreach (Renderer renderer in this._powerCables)
            this._materials.Add(renderer.material);
    }

    private void Start()
    {
        this.Colorize();
        if (this._locked)
            return;
        if (this._open)
            this._playAnimation.Play(0);
        else
            this._playAnimation.Play(1);
        this._collider.isTrigger = this._open;
        this._interactable.SetCustomText(this._open ? GameConstants.INTERACT_DOOR_CLOSE : GameConstants.INTERACT_DOOR_OPEN);
    }

    public void Change()
    {
        if (this._locked)
        {
            this._onLocked.Invoke();
            return;
        }
        this._open = !this._open;
        this._playAnimation.PlayNext();
        this._collider.isTrigger = this._open;
        this._interactable.SetCustomText(this._open ? GameConstants.INTERACT_DOOR_CLOSE : GameConstants.INTERACT_DOOR_OPEN);
    }

    public void Unlock()
    {
        this._locked = false;
        this.Colorize();
    }

    private void Colorize()
    {
        if (!this._locked)
        {
            this._material.color = this._unlockedColor;
            foreach (Material material in this._materials)
                material.color = this._unlockedColor;
        }
        else
        {
            this._material.color = this._lockedColor;
            foreach (Material material in this._materials)
                material.color = this._lockedColor;
        }
    }
}
