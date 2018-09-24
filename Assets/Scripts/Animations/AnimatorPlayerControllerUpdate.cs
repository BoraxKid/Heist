using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorPlayerControllerUpdate : MonoBehaviour
{
    [SerializeField] private AnimationCurve _speedMapping = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Animator _animator;

    private void Awake()
    {
        this._animator = this.GetComponent<Animator>();
        this._animator.SetBool("OnGround", true);
    }

    private void LateUpdate()
    {
        if (GameConstants.paused)
            return;
        Vector3 tmp = GameConstants.playerVariable.movement;
        tmp.y = 0.0f;
        if (tmp.magnitude > 1.0f)
            tmp.Normalize();
        tmp = this.transform.InverseTransformDirection(tmp);
        tmp = Vector3.ProjectOnPlane(tmp, Vector3.up);

        this._animator.SetFloat("Forward", this._speedMapping.Evaluate(GameConstants.playerVariable.movementInput.magnitude));
        this._animator.SetFloat("Turn", Mathf.Atan2(tmp.x, tmp.z));
        this._animator.SetBool("OnGround", GameConstants.playerVariable.isGrounded);
        this._animator.SetBool("Crouch", GameConstants.playerVariable.isCrouched);
        this._animator.SetFloat("Jump", GameConstants.playerVariable.velocity.y);
    }
}
