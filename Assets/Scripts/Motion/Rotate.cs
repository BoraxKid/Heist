using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private AnimationCurve _rotationCurve;
    [SerializeField] private Vector3 _constantForce;

    private void FixedUpdate()
    {
        if (GameConstants.paused)
            return;
        if (this._constantForce != Vector3.zero)
            this.transform.Rotate(this._constantForce);
    }

    public void SingleTimeRotationY(float force)
    {
        this.StartCoroutine(this.Rotation(new Vector3(0.0f, force, 0.0f)));
    }

    private IEnumerator Rotation(Vector3 force)
    {
        float elapsedTime = 0.0f;
        float time = this._rotationCurve.keys[this._rotationCurve.length - 1].time;

        while (elapsedTime < time)
        {
            if (GameConstants.paused)
            {
                yield return null;
                continue;
            }
            this.transform.Rotate(force * this._rotationCurve.Evaluate(elapsedTime));
            yield return null;
            elapsedTime += Time.deltaTime;
        }
        yield break;
    }
}
