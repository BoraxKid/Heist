using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ChromaticAberrationModifier : MonoBehaviour
{
    [SerializeField] private AnimationCurve _aberrationCurve;
    [SerializeField] private PostProcessProfile _profile;
    [SerializeField] private FloatReference _smoothTime;
    [SerializeField] private FloatReference _originalIntensity;

    private ChromaticAberration _chromaticAberration;

    private bool _originalActive;

    private void Awake()
    {
        if (!this._profile.TryGetSettings<ChromaticAberration>(out this._chromaticAberration))
            Debug.LogWarning("Can't find a ChromaticAberration setting");
        this._originalActive = this._chromaticAberration.active;
        this._chromaticAberration.intensity.value = this._originalIntensity.Value;
    }

    private void OnDestroy()
    {
        this._chromaticAberration.active = this._originalActive;
        this.SetIntensity(this._originalIntensity.Value);
    }

    public void Reset()
    {
        this._chromaticAberration.active = this._originalActive;
        this.SetIntensitySmooth(this._originalIntensity.Value);
    }

    public void Evaluate(float value)
    {
        this._chromaticAberration.intensity.value = this._aberrationCurve.Evaluate(value);
    }

    public void SetIntensity(float value)
    {
        this._chromaticAberration.intensity.value = value;
    }

    public void SetIntensitySmooth(float value)
    {
        this.StartCoroutine(this.SmoothIntensity(value));
    }

    private IEnumerator SmoothIntensity(float value)
    {
        float originalValue = this._chromaticAberration.intensity.value;
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
            this._chromaticAberration.intensity.value = Mathf.Lerp(originalValue, value, elapsedTime / this._smoothTime.Value);
            yield return null;
            elapsedTime += Time.deltaTime;
        }
        this._chromaticAberration.intensity.value = value;
        yield break;
    }
}
