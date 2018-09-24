using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialModifier : MonoBehaviour
{
    [SerializeField] private AnimationCurve _emissionCurve;

    private Material _material;
    private Color _emissionColor;

    private void Awake()
    {
        this._material = this.GetComponent<Renderer>().material;
        this._emissionColor = this._material.GetColor("_EmissionColor");
    }

    public void AnimateEmission()
    {
        this.StopAllCoroutines();
        this.StartCoroutine(this.Animation());
    }

    private IEnumerator Animation()
    {
        float elapsedTime = 0.0f;
        float time = this._emissionCurve.keys[this._emissionCurve.length - 1].time;

        while (elapsedTime < time)
        {
            if (GameConstants.paused)
            {
                yield return null;
                continue;
            }
            this._material.SetColor("_EmissionColor", this._emissionColor * this._emissionCurve.Evaluate(elapsedTime));
            yield return null;
            elapsedTime += Time.deltaTime;
        }
        yield break;
    }
}
