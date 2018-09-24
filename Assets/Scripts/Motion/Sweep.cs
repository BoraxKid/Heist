using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sweep : MonoBehaviour
{
    [SerializeField] private AnimationCurve _rotationCurve;
    [SerializeField] private Vector3 _axis;
    [SerializeField] private float _speed = 0.5f;
    [SerializeField] private float _amplitude = 0.75f;

    private float _elapsedTime;
    private Quaternion _originalRotation;

    private void Awake()
    {
        this._originalRotation = this.transform.rotation;
    }

    private void OnEnable()
    {
        this.transform.rotation = this._originalRotation;
    }

    private void OnDisable()
    {
        this._originalRotation = this.transform.rotation;
    }

    private void FixedUpdate()
    {
        if (GameConstants.paused)
            return;
        this._elapsedTime += Time.fixedDeltaTime * this._speed;
        this.transform.Rotate(this._axis, this._rotationCurve.Evaluate(this._elapsedTime) * this._amplitude);
    }
}
