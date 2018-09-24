using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
public class ChangeScale : MonoBehaviour
{
    [SerializeField] private FloatReference _normalScale;
    [SerializeField] private FloatReference _alternateScale;
    [SerializeField] private AnimationCurve _scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private FloatReference _animationTime;
    [SerializeField] private UnityEvent _endAnimationEvent;
    [SerializeField] private bool _scaleX;
    [SerializeField] private bool _scaleY;

    private RectTransform _rectTransform;

    private void Awake()
    {
        this._rectTransform = this.GetComponent<RectTransform>();
        this._rectTransform.localScale = new Vector3((this._scaleX ? this._normalScale.Value : this._rectTransform.localScale.x), (this._scaleY ? this._normalScale.Value : this._rectTransform.localScale.y), this._rectTransform.localScale.z);
    }

    public void SetNormalScale()
    {
        this.StopAllCoroutines();
        this.StartCoroutine(this.AnimateScale(false));
    }

    public void SetAlternateScale()
    {
        this.StopAllCoroutines();
        this.StartCoroutine(this.AnimateScale(true));
    }

    private IEnumerator AnimateScale(bool alternate)
    {
        float elapsedTime = 0;
        float tmp = this._rectTransform.localScale.y;
        Vector3 scale = this._rectTransform.localScale;
        float scaleFrom = (alternate) ? this._normalScale.Value : this._alternateScale.Value;
        float scaleTo = (alternate) ? this._alternateScale.Value : this._normalScale.Value;

        if ((this._scaleX && scale.x == scaleTo) || (this._scaleY && scale.y == scaleTo))
        {
            this._endAnimationEvent.Invoke();
            yield break;
        }

        elapsedTime = Mathf.InverseLerp(scaleFrom, scaleTo, tmp) * this._animationTime.Value;

        while (elapsedTime <= this._animationTime.Value)
        {
            // if (GameConstants.paused)
            // {
            //     yield return null;
            //     continue;
            // }
            tmp = Mathf.Lerp(scaleFrom, scaleTo, this._scaleCurve.Evaluate(elapsedTime / this._animationTime.Value));
            this._rectTransform.localScale = new Vector3((this._scaleX ? tmp : scale.x), (this._scaleY ? tmp : scale.y), scale.z);
            yield return null;
            elapsedTime += Time.deltaTime;
        }
        this._rectTransform.localScale = new Vector3((this._scaleX ? scaleTo : scale.x), (this._scaleY ? scaleTo : scale.y), scale.z);
        this._endAnimationEvent.Invoke();
        yield break;
    }
}
