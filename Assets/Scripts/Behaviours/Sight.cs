using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sight : MonoBehaviour
{
    [Header("Constants")]
    [SerializeField] private ParticleSystem _detectionParticleSystemPrefab;

    [Header("Properties")]
    [SerializeField] private LayerMask _sightLayerMask;
    [SerializeField] private FloatReference _confirmDetectionTime;
    [SerializeField] private Transform _eye;
    [SerializeField] private Vector3 _eyeRotationCorrection = Vector3.forward;
    [SerializeField] private FloatReference _range;
    [SerializeField] private FloatReference _FOV;

    [Header("Events")]
    [SerializeField] private UnityEvent _onPlayerDetectionStart;
    [SerializeField] private FloatUnityEvent _onPlayerDetectionUpdate;
    [SerializeField] private UnityEvent _onPlayerDetectionFailed;
    [SerializeField] private UnityEvent _onPlayerDetection;
    [SerializeField] private UnityEvent _onPlayerLost;

    public bool QuickDetect
    {
        get;
        set;
    }

    public float DetectProgress
    {
        get;
        private set;
    }

    private bool __seing;
    private bool _seeing
    {
        get
        {
            return (this.__seing);
        }
        set
        {
            if (this.__seing != value)
            {
                if (value)
                {
                    if (this.QuickDetect)
                        this.Detect(true);
                    else if (this._confirmDetectionCoroutine == null)
                        this._confirmDetectionCoroutine = this.StartCoroutine(this.ConfirmDetection());
                }
                else
                {
                    if (!this._detected)
                        this._onPlayerDetectionFailed.Invoke();
                    if (this._confirmDetectionCoroutine != null)
                        this.StopCoroutine(this._confirmDetectionCoroutine);
                    this._confirmDetectionCoroutine = null;
                    if (this._detected)
                        this.Detect(false);
                }
                this.__seing = value;
            }
        }
    }

    private bool _detected;
    private Coroutine _confirmDetectionCoroutine;
    private HeadLookController _headLookController;
    private Quaternion _defaultRotation;

    private void Awake()
    {
        if (this._eye == null)
            this._eye = this.transform;
        this._seeing = false;
        this._headLookController = this.GetComponent<HeadLookController>();
        this._defaultRotation = this._eye.transform.rotation;
    }

    private void OnDisable()
    {
        this._onPlayerDetectionFailed.Invoke();
        this.StopAllCoroutines();
        this.CancelInvoke();
    }

    private void Update()
    {
        if (GameConstants.paused)
            return;
        //Debug.DrawRay(this._eye.position, this._eye.TransformDirection(this._eyeRotationCorrection).normalized * this._range.Value, Color.red);
        this._seeing = this.Look();
    }

    private bool Look()
    {
        Vector3 targetCenter = Helper.GetCenter(GameConstants.playerVariable.gameObject);
        Vector3 targetTop = Helper.GetTop(GameConstants.playerVariable.gameObject);
        Vector3 targetBottom = Helper.GetBottom(GameConstants.playerVariable.gameObject);

        //this.DebugDrawLook(targetTop, targetCenter, targetBottom);

        return (this.CheckVisibility(targetTop) || this.CheckVisibility(targetCenter) || this.CheckVisibility(targetBottom));
    }

    private void DebugDrawLook(Vector3 targetTop, Vector3 targetCenter, Vector3 targetBottom)
    {
        //DebugExtension.DebugCone(this._eye.position, this._eye.TransformDirection(this._eyeRotationCorrection) * this._range.Value, this._seeing ? Color.yellow : Color.black, this._FOV.Value / 2.0f);
    }

    private bool CheckVisibility(Vector3 target)
    {
        if (Vector3.Distance(target, this._eye.position) > this._range.Value)
        {
            //Debug.Log("Distance fail");
            return (false);
        }

        RaycastHit hitInfo;
        if (Physics.Linecast(this._eye.position, target, out hitInfo, this._sightLayerMask.value) && hitInfo.collider.gameObject != GameConstants.playerVariable.gameObject)
        {
            //Debug.Log("Linecast fail, hit " + hitInfo.collider.name);
            return (false);
        }

        Vector3 targetDirection = target - this._eye.position;

        float angleToTarget = Vector3.Angle(targetDirection, this._eye.TransformDirection(this._eyeRotationCorrection));

        if (angleToTarget <= this._FOV.Value / 2.0f)
            return (true);
        //Debug.Log("FOV fail");
        return (false);
    }

    private void Detect(bool detected)
    {
        if (!detected)
        {
            this._onPlayerLost.Invoke();
        }
        else
        {
            this._onPlayerDetection.Invoke();
            Helper.SpawnParticlesSystemOnTop(this._detectionParticleSystemPrefab, this.gameObject);
        }
        this._detected = detected;
    }

    private IEnumerator ConfirmDetection()
    {
        float elapsedTime = 0.0f;
        float lastDeltaTime = 0.0f;
        float time = Mathf.Max(GameConstants.TIME_MIN_DETECTION, this._confirmDetectionTime.Value * (1.0f - GameConstants.playerVariable.brightness));

        this._onPlayerDetectionStart.Invoke();
        this.DetectProgress = 0.0f;

        while (elapsedTime <= time)
        {
            if (GameConstants.paused)
            {
                yield return null;
                continue;
            }
            time = Mathf.Max(GameConstants.TIME_MIN_DETECTION, this._confirmDetectionTime.Value * (1.0f - GameConstants.playerVariable.brightness));
            this.DetectProgress += lastDeltaTime / time;
            this._onPlayerDetectionUpdate.Invoke(this.DetectProgress);
            yield return null;
            lastDeltaTime = Time.deltaTime;
            elapsedTime += Time.deltaTime;
        }
        this.DetectProgress = 1.0f;
        this._onPlayerDetectionUpdate.Invoke(1.0f);
        this.Detect(true);
        yield break;
    }
}
