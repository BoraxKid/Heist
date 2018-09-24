using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Search : MonoBehaviour
{
    [Header("Particles")]
    [SerializeField] private ParticleSystem _suspicionParticleSystemPrefab;
    [SerializeField] private ParticleSystem _lostParticleSystemPrefab;

    [Header("Properties")]
    [SerializeField] private FloatReference _lostTrackTime;
    [SerializeField] private FloatReference _searchTime;
    [SerializeField] private FloatReference _changePositionTime;
    [SerializeField] private FloatReference _searchRadius;
    [SerializeField] private FloatReference _walkingSpeed;

    [Header("Events")]
    [SerializeField] private UnityEvent _onStopSearch;

    private NavMeshAgent _navMeshAgent;
    private Vector3 _lastSeenPosition;
    private float _elapsedTime;
    private float _changePositionElapsedTime;
    private bool _lostTrack;

    private void Awake()
    {
        this._navMeshAgent = this.GetComponent<NavMeshAgent>();
        this.SetWalkingSpeed();
    }

    private void OnEnable()
    {
        this._lastSeenPosition = GameConstants.playerVariable.gameObject.transform.position + GameConstants.playerVariable.gameObject.transform.forward * this._searchRadius.Value;
        DebugExtension.DebugArrow(GameConstants.playerVariable.gameObject.transform.position, this._lastSeenPosition - GameConstants.playerVariable.gameObject.transform.position, Color.green, 5.0f);
        this._navMeshAgent.SetDestination(this._lastSeenPosition);
        this._elapsedTime = 0.0f;
        this._changePositionElapsedTime = 0.0f;
        this._lostTrack = false;
    }

    private void Update()
    {
        if (GameConstants.paused)
            return;
        this._elapsedTime += Time.deltaTime;
        if (!this._lostTrack)
        {
            if (this._elapsedTime >= this._lostTrackTime.Value)
            {
                this._elapsedTime -= this._lostTrackTime.Value;
                this._lostTrack = true;
                this.SetWalkingSpeed();
                Helper.SpawnParticlesSystemOnTop(this._suspicionParticleSystemPrefab, this.gameObject);
            }
            return;
        }
        DebugExtension.DebugCircle(this._lastSeenPosition, Color.red, this._searchRadius.Value);
        if (this._elapsedTime >= this._searchTime.Value)
        {
            Helper.SpawnParticlesSystemOnTop(this._lostParticleSystemPrefab, this.gameObject);
            this._onStopSearch.Invoke();
            return;
        }
        this._changePositionElapsedTime += Time.deltaTime;
        if (this._changePositionElapsedTime >= this._changePositionTime.Value)
        {
            this._changePositionElapsedTime -= this._changePositionTime.Value;
            this._navMeshAgent.SetDestination(Helper.RandomNavSphere(this._lastSeenPosition, this._searchRadius.Value));
        }
    }

    private void SetWalkingSpeed()
    {
        this._navMeshAgent.speed = this._walkingSpeed.Value;
        this._navMeshAgent.acceleration = this._walkingSpeed.Value;
    }
}
