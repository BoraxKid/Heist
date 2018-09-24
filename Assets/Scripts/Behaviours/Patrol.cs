using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class Patrol : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private Vector3List _waypoints;
    [SerializeField] private FloatReference _waitAtPointTime;

    [Header("Events")]
    [SerializeField] private UnityEvent _onReachWaypoint;
    [SerializeField] private UnityEvent _onLeaveWaypoint;

    private NavMeshAgent _navMeshAgent;
    private bool __moving;
    private bool _moving
    {
        get
        {
            return (this.__moving);
        }
        set
        {
            if (value != this.__moving)
            {
                if (value)
                    this._onLeaveWaypoint.Invoke();
                else
                    this._onReachWaypoint.Invoke();
                this.__moving = value;
            }
        }
    }
    private float _elapsedTime = 0.0f;
    private int _targetWaypointIndex = -1;

    private void Awake()
    {
        this._navMeshAgent = this.GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        this.GoToClosestDestination();
        this.InvokeRepeating("PatrolUpdate", 0.33f, 0.33f);
    }

    private void OnDisable()
    {
        this.CancelInvoke("PatrolUpdate");
    }

    private void Update()
    {
        if (GameConstants.paused)
            return;
        this._elapsedTime += Time.deltaTime;
    }

    private void PatrolUpdate()
    {
        if (GameConstants.paused)
            return;
        if (this._moving && this._navMeshAgent.remainingDistance <= this._navMeshAgent.stoppingDistance)
        {
            this._moving = false;
            this._elapsedTime = 0.0f;
        }
        else if (!this._moving && this._elapsedTime >= this._waitAtPointTime.Value)
        {
            this.ChangeDestination();
        }
    }

    public void GoToClosestDestination()
    {
        this._targetWaypointIndex = this.GetClosestDestination() - 1; // Get index of closest waypoint minus 1 since ChangeDestination preincrement before using the index
        this.ChangeDestination();
    }

    private void ChangeDestination()
    {
        if (this._targetWaypointIndex + 1 >= this._waypoints.Value.Count)
            this._targetWaypointIndex = 0;
        else
            ++this._targetWaypointIndex;
        this._moving = true;
        this._navMeshAgent.SetDestination(this._waypoints.Value[this._targetWaypointIndex]);
        this._elapsedTime = 0.0f;
    }

    private int GetClosestDestination()
    {
        int waypointIndex = 0;
        float range = float.MaxValue;
        for (int i = 0; i < this._waypoints.Value.Count; ++i)
        {
            if (Vector3.Distance(this.transform.position, this._waypoints.Value[i]) < range)
            {
                waypointIndex = i;
                range = Vector3.Distance(this.transform.position, this._waypoints.Value[i]);
            }
        }
        return (waypointIndex);
    }
}
