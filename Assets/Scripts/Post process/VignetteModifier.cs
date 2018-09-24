using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class VignetteModifier : MonoBehaviour
{
    [SerializeField] private AnimationCurve _curve;
    [SerializeField] private PostProcessProfile _profile;
    [SerializeField] private FloatReference _smoothTime;
    [SerializeField] private FloatReference _originalIntensity;

    private Vignette _vignette;
    private bool _originalActive;

    private void Awake()
    {
        if (!this._profile.TryGetSettings<Vignette>(out this._vignette))
            Debug.LogWarning("Can't find a Vignette setting");
        this._originalActive = this._vignette.active;
        this._vignette.intensity.value = this._originalIntensity.Value;
    }

    private void OnDestroy()
    {
        this._vignette.active = this._originalActive;
        this.SetIntensity(this._originalIntensity.Value);
    }

    public void Reset()
    {
        this._vignette.active = this._originalActive;
        this.SetIntensitySmooth(this._originalIntensity.Value);
    }

    public void Evaluate(float value)
    {
        this._vignette.intensity.value = Mathf.Max(this._vignette.intensity.value, this._curve.Evaluate(value));
    }

    public void SetIntensity(float value)
    {
        this._vignette.intensity.value = value;
    }

    public void SetIntensitySmooth(float value)
    {
        this.StopAllCoroutines();
        this.StartCoroutine(this.SmoothIntensity(value));
    }

    private IEnumerator SmoothIntensity(float value)
    {
        float originalValue = this._vignette.intensity.value;
        if (originalValue == value)
            yield break;
        float elapsedTime = 0.0f;

        while (elapsedTime <= this._smoothTime.Value)
        {
            if (GameConstants.paused)
            {
                yield return null;
                continue;
            }
            this._vignette.intensity.value = Mathf.Lerp(originalValue, value, elapsedTime / this._smoothTime.Value);
            yield return null;
            elapsedTime += Time.deltaTime;
        }
        this._vignette.intensity.value = value;
        yield break;
    }
}
