using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class AnimatorNavMeshAgentUpdate : MonoBehaviour
{
    [SerializeField] private AnimationCurve _speedMapping;

    private Animator _animator;
    private NavMeshAgent _navMeshAgent;

    private void Awake()
    {
        this._animator = this.GetComponent<Animator>();
        this._navMeshAgent = this.GetComponent<NavMeshAgent>();
    }

    private void LateUpdate()
    {
        if (GameConstants.paused)
            return;
        Vector3 velocity = this._navMeshAgent.velocity;
        Vector3 normalizedVelocity = velocity.normalized;

        normalizedVelocity = this.transform.InverseTransformDirection(normalizedVelocity);
        normalizedVelocity = Vector3.ProjectOnPlane(normalizedVelocity, Vector3.up);
        this._animator.SetFloat("Forward", this._speedMapping.Evaluate(velocity.magnitude));
        this._animator.SetFloat("Turn", Mathf.Atan2(normalizedVelocity.x, normalizedVelocity.z));
    }
}
