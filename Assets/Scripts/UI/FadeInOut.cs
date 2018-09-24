using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadeInOut : MonoBehaviour
{
    [SerializeField] private AnimationCurve _fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private FloatReference _fadeTime;

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        this._canvasGroup = this.GetComponent<CanvasGroup>();
    }

    public void Show()
    {
        this.StartCoroutine(this.Fade(true));
    }

    public void Hide()
    {
        this.StartCoroutine(this.Fade(false));
    }

    private IEnumerator Fade(bool fadeIn)
    {
        float elapsedTime = 0.0f;

        if ((fadeIn && this._canvasGroup.alpha == 1.0f) || (!fadeIn && this._canvasGroup.alpha == 0.0f))
            yield break;

        while (elapsedTime <= this._fadeTime.Value)
        {
            // if (GameConstants.paused)
            // {
            //     yield return null;
            //     continue;
            // }
            if (fadeIn)
                this._canvasGroup.alpha = this._fadeCurve.Evaluate(elapsedTime / this._fadeTime.Value);
            else
                this._canvasGroup.alpha = this._fadeCurve.Evaluate(1.0f - elapsedTime / this._fadeTime.Value);
            yield return null;
            elapsedTime += Time.deltaTime;
        }
        this._canvasGroup.alpha = this._fadeCurve.Evaluate((fadeIn) ? 1.0f : 0.0f);
        yield break;
    }
}
