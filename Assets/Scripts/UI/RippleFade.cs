using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class RippleFade : MonoBehaviour
{
    [SerializeField] private AnimationCurve _effectCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private FloatReference _effectTime;
    [SerializeField] private FloatReference _dilationFactor;
    [SerializeField] private UnityEvent _onComplete;

    private Image _image;
    private RectTransform _rectTransform;
    private Color _color;
    private Vector3 _scale;

    private void Awake()
    {
        this._image = this.GetComponent<Image>();
        this._rectTransform = this.GetComponent<RectTransform>();
        this._color = this._image.color;
        this._scale = this._rectTransform.localScale;
    }

    public void Reset()
    {
        this._image.color = this._color;
        this._rectTransform.localScale = this._scale;
    }

    public void PlayEffect()
    {
        this.StartCoroutine(this.Play());
    }

    private IEnumerator Play()
    {
        float elapsedTime = 0.0f;
        float value;
        Color color = this._color;
        Vector3 originalScale = this._rectTransform.localScale;

        while (elapsedTime <= this._effectTime.Value)
        {
            if (GameConstants.paused)
            {
                yield return null;
                continue;
            }
            value = this._effectCurve.Evaluate(elapsedTime / this._effectTime.Value);
            color.a = 1.0f - value;
            this._image.color = color;
            this._rectTransform.localScale = new Vector3(originalScale.x + value * this._dilationFactor.Value, originalScale.y + value * this._dilationFactor.Value, originalScale.z);
            yield return null;
            elapsedTime += Time.deltaTime;
        }
        value = this._effectCurve.Evaluate(1.0f);
        color.a = 1.0f - value;
        this._image.color = color;
        this._rectTransform.localScale = new Vector3(originalScale.x + value * this._dilationFactor.Value, originalScale.y + value * this._dilationFactor.Value, originalScale.z);
        this._onComplete.Invoke();
        yield break;
    }
}
