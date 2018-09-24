using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class PlayAnimation : MonoBehaviour
{
    [SerializeField] private List<string> _animationNames;

    private Animation _animation;
    private int _nextAnimationIndex;

    private void Awake()
    {
        this._animation = this.GetComponent<Animation>();
        this._nextAnimationIndex = 0;
    }

    public void Play(int animationIndex)
    {
        this._nextAnimationIndex = animationIndex;
        this.PlayNext();
    }

    public void PlayNext()
    {
        this._animation.Play(this._animationNames[this._nextAnimationIndex]);
        ++this._nextAnimationIndex;
        if (this._nextAnimationIndex >= this._animationNames.Count)
            this._nextAnimationIndex = 0;
    }
}
