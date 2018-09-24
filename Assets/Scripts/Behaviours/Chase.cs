using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Chase : MonoBehaviour
{
    [SerializeField] private FloatReference _runningSpeed;

    private NavMeshAgent _navMeshAgent;

    private void Awake()
    {
        this._navMeshAgent = this.GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        this._navMeshAgent.speed = this._runningSpeed.Value;
        this._navMeshAgent.acceleration = this._runningSpeed.Value;
    }

    private void Update()
    {
        if (GameConstants.paused)
            return;
        this._navMeshAgent.SetDestination(GameConstants.playerVariable.gameObject.transform.position);
    }
}
