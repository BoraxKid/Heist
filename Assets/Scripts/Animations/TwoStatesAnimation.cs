using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayAnimation))]
public class TwoStatesAnimation : MonoBehaviour
{
    private PlayAnimation _playAnimation;
    [SerializeField] private bool _state = false;

    private void Awake()
    {
        this._playAnimation = this.GetComponent<PlayAnimation>();
    }

    public void ChangeState()
    {
        this._state = !this._state;
        this._playAnimation.Play(this._state ? 1 : 0);
    }
}
