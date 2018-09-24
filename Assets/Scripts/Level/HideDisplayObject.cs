using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class HideDisplayObject : MonoBehaviour
{
    [SerializeField] private Material _hideMaterial;

    private bool _visible;
    private MeshRenderer _meshRenderer;
    private Material _defaultMaterial;

    private void Awake()
    {
        this._meshRenderer = this.GetComponent<MeshRenderer>();
        this._defaultMaterial = this._meshRenderer.material;
        this._visible = true;
    }

    public void ChangeState(bool display, float changeTime)
    {
        if (display && !this._visible)
        {
            this.StopAllCoroutines();
            this.StartCoroutine(this.DisplayObject(changeTime));
        }
        else if (!display && this._visible)
        {
            this.StopAllCoroutines();
            this.StartCoroutine(this.HideObject(changeTime));
        }
    }

    private IEnumerator DisplayObject(float changeTime)
    {
        float elapsedTime = 0.0f;

        this._meshRenderer.enabled = true;
        Material currentMaterial = this._meshRenderer.material;
        this._visible = true;
        while (elapsedTime <= changeTime)
        {
            if (GameConstants.paused)
            {
                yield return null;
                continue;
            }
            currentMaterial.SetFloat("_Opacity", elapsedTime / changeTime);
            yield return null;
            elapsedTime += Time.deltaTime;
        }
        currentMaterial.SetFloat("_Opacity", 1.0f);
        this._meshRenderer.material = this._defaultMaterial;

        yield break;
    }

    private IEnumerator HideObject(float changeTime)
    {
        float elapsedTime = 0.0f;

        this._meshRenderer.material = this._hideMaterial;
        Material currentMaterial = this._meshRenderer.material;
        this._visible = false;
        while (elapsedTime <= changeTime)
        {
            if (GameConstants.paused)
            {
                yield return null;
                continue;
            }
            currentMaterial.SetFloat("_Opacity", 1.0f - (elapsedTime / changeTime));
            yield return null;
            elapsedTime += Time.deltaTime;
        }
        currentMaterial.SetFloat("_Opacity", 0.0f);
        this._meshRenderer.enabled = false;
        yield break;
    }
}
