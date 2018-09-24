using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class AnimateText : MonoBehaviour
{
    [SerializeField] private AnimationCurve _vertexCurve;
    [SerializeField] private FloatReference _animationScale;
    [SerializeField] private FloatReference _animationDuration;
    [SerializeField] private FloatReference _animationSpeed;
    [SerializeField] private UnityEvent _endAnimationEvent;

    private TextMeshProUGUI _textMesh;

    private void Awake()
    {
        this._textMesh = this.GetComponent<TextMeshProUGUI>();
    }

    public void StartAnimation()
    {
        this.StopAllCoroutines();
        this.StartCoroutine(this.AnimateWave());
    }

    public void StopAnimation()
    {
        this.StopAllCoroutines();
        this._textMesh.ForceMeshUpdate();
    }

    private IEnumerator AnimateWave()
    {
        TMP_TextInfo textInfo = this._textMesh.textInfo;
        TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
        int characterCount = textInfo.characterCount;
        TMP_CharacterInfo characterInfo;
        int materialIndex;
        int vertexIndex;
        float elapsedTime = 0;

        while (elapsedTime < this._animationDuration.Value * this._animationSpeed.Value)
        {
            for (int i = 0; i < characterCount; ++i)
            {
                characterInfo = textInfo.characterInfo[i];
                if (!characterInfo.isVisible)
                    continue;
                materialIndex = characterInfo.materialReferenceIndex;
                vertexIndex = characterInfo.vertexIndex;
                Vector3[] srcVertices = cachedMeshInfo[materialIndex].vertices;
                Vector3[] dstVertices = textInfo.meshInfo[materialIndex].vertices;
                Vector3 offset = this.OffsetWave(elapsedTime, i, characterCount);

                for (int j = 0; j < 4; ++j)
                    dstVertices[vertexIndex + j] = srcVertices[vertexIndex + j] + offset;
            }

            for (int i = 0; i < textInfo.meshInfo.Length; ++i)
            {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                this._textMesh.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }
            yield return null;
            elapsedTime += Time.deltaTime * this._animationSpeed.Value;
        }
        this._endAnimationEvent.Invoke();
        yield break;
    }

    private Vector3 OffsetWave(float elapsedTime, int i, int count)
    {
        return (new Vector3(0, this._vertexCurve.Evaluate(elapsedTime - i * (this._vertexCurve.keys[this._vertexCurve.keys.Length - 1].time / count)) * this._animationScale.Value, 0));
    }
}
