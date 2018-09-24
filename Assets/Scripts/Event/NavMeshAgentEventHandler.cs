using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshAgentEventHandler : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private Vector3 _pausedVelocity;

    private void Awake()
    {
        this._navMeshAgent = this.GetComponent<NavMeshAgent>();
    }

    public void OnPause()
    {
        if (!this._navMeshAgent.enabled)
            return;
        this._navMeshAgent.isStopped = true;
        this._pausedVelocity = this._navMeshAgent.velocity;
        this._navMeshAgent.velocity = Vector3.zero;
    }

    public void OnResume()
    {
        if (!this._navMeshAgent.enabled)
            return;
        this._navMeshAgent.velocity = this._pausedVelocity;
        this._navMeshAgent.isStopped = false;
    }
}
