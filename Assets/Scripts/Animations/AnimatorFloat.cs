using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorFloat : MonoBehaviour
{
    [SerializeField] private string _parameterName;
    [SerializeField] private AnimationCurve _curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Animator _animator;

    private void Awake()
    {
        this._animator = this.GetComponent<Animator>();
    }

    public void To(float value)
    {
        this.StartCoroutine(this.ModifyValue(this._animator.GetFloat(this._parameterName), value));
    }

    private IEnumerator ModifyValue(float from, float to)
    {
        float elapsedTime = 0.0f;
        float time = this._curve.keys[this._curve.length - 1].time;

        while (elapsedTime <= time)
        {
            if (GameConstants.paused)
            {
                yield return null;
                continue;
            }
            if (to == 1.0f)
                this._animator.SetFloat(this._parameterName, this._curve.Evaluate(elapsedTime));
            else if (to == 0.0f)
                this._animator.SetFloat(this._parameterName, 1 - this._curve.Evaluate(elapsedTime));
            yield return null;
            elapsedTime += Time.deltaTime;
        }
        this._animator.SetFloat(this._parameterName, to);
        yield break;
    }
}
