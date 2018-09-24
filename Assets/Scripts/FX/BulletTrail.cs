using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BulletTrail : MonoBehaviour
{
    [SerializeField] private Vector3 _startPoint;
    [SerializeField] private Vector3 _endPoint;
    [SerializeField] private FloatReference _timeBeforeStartDisappearing;
    [SerializeField] private AnimationCurve _animationCurve;

    private LineRenderer _lineRenderer;

    private void Awake()
    {
        if (GameConstants.bulletTrailPool == null)
            GameConstants.bulletTrailPool = this.GetComponentInParent<PrefabPool>();
        this._lineRenderer = this.GetComponent<LineRenderer>();
    }

    public void SetStartPoint(Vector3 startPoint)
    {
        this._startPoint = startPoint;
    }

    public void SetEndPoint(Vector3 endPoint)
    {
        this._endPoint = endPoint;
    }

    public void SetPoints(Vector3 startPoint, Vector3 endPoint)
    {
        this._startPoint = startPoint;
        this._endPoint = endPoint;
    }

    public void Display()
    {
        this.StartCoroutine(this.Animate());
    }

    private IEnumerator Animate()
    {
        float elapsedTime = 0.0f;
        float time = this._animationCurve.keys[this._animationCurve.length - 1].time;

        this._lineRenderer.positionCount = 2;
        this._lineRenderer.SetPosition(0, this._startPoint);
        this._lineRenderer.SetPosition(1, this._endPoint);
        this._lineRenderer.enabled = true;

        while (elapsedTime <= this._timeBeforeStartDisappearing.Value)
        {
            if (GameConstants.paused)
            {
                yield return null;
                continue;
            }
            yield return null;
            elapsedTime += Time.deltaTime;
        }
        elapsedTime = 0.0f;
        while (elapsedTime <= time)
        {
            if (GameConstants.paused)
            {
                yield return null;
                continue;
            }
            this._lineRenderer.SetPosition(0, Vector3.Lerp(this._startPoint, this._endPoint, this._animationCurve.Evaluate(elapsedTime)));
            yield return null;
            elapsedTime += Time.deltaTime;
        }
        this._lineRenderer.SetPosition(0, this._endPoint);
        this._lineRenderer.enabled = false;
        GameConstants.bulletTrailPool.Release(this.gameObject);
        yield break;
    }
}
