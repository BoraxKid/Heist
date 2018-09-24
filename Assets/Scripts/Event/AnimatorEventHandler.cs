using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorEventHandler : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        this._animator = this.GetComponent<Animator>();
        this.enabled = false;
    }

    public void OnPause()
    {
        this._animator.enabled = false;
    }

    public void OnResume()
    {
        this._animator.enabled = true;
    }
}
