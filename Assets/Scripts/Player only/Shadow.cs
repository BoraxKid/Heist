using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Shadow : MonoBehaviour
{
    [SerializeField] private bool _bakedLighting;
    [SerializeField] private AnimationCurve _brightnessLevelCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve _lightIntensityOverDistanceCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    [SerializeField] private FloatReference _crouchModifierFactor;
    [SerializeField] private FloatReference _radius;
    [SerializeField] private LayerMask _lightLayerMask;
    [SerializeField] private FloatReference _updateTime;

    private Vector3[] _directions;
    private Color[] _colors;
    private Renderer _renderer;
    private SphericalHarmonicsL2 _probe;

    private void Awake()
    {
        this._renderer = this.GetComponentInChildren<Renderer>();
        this._directions = new Vector3[1];
        this._directions[0] = this.transform.up;
        this._colors = new Color[1];
        this.InvokeRepeating("BrightnessUpdater", this._updateTime.Value, this._updateTime.Value);
    }

    private void BrightnessUpdater()
    {
        if (GameConstants.paused)
            return;

        if (this._bakedLighting)
        {
            LightProbes.GetInterpolatedProbe(Helper.GetCenter(this.gameObject), this._renderer, out this._probe);
            this._probe.Evaluate(this._directions, this._colors);
            GameConstants.playerVariable.brightness = this._brightnessLevelCurve.Evaluate(this._colors[0].grayscale * (GameConstants.playerVariable.isCrouched ? this._crouchModifierFactor.Value : 1.0f));
        }
        else
        {
            Vector3 origin = Helper.GetCenter(this.gameObject);
            Collider[] hits = Physics.OverlapSphere(origin, this._radius.Value, this._lightLayerMask);
            RaycastHit hitInfo;
            Light light;
            float brightness = 0.0f;
            float tmp;

            foreach (Collider hit in hits)
            {
                tmp = Vector3.Distance(origin, hit.transform.position);

                if (tmp <= this._radius.Value && !Physics.Linecast(origin, hit.transform.position, out hitInfo, Physics.DefaultRaycastLayers & ~(1 << LayerMask.NameToLayer("Player")) & ~(this._lightLayerMask.value)))
                {
                    light = hit.GetComponent<Light>();
                    tmp = this.LightBrightness(tmp, light.intensity, light.range);
                    brightness = (tmp > brightness) ? tmp : brightness;
                }
            }

            GameConstants.playerVariable.brightness = this._brightnessLevelCurve.Evaluate(brightness * (GameConstants.playerVariable.isCrouched ? this._crouchModifierFactor.Value : 1.0f));
        }
    }

    private float LightBrightness(float distance, float lightIntensity, float lightRange)
    {
        return (this._lightIntensityOverDistanceCurve.Evaluate(distance / lightRange) * lightIntensity);
    }
}
