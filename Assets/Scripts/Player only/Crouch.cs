using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crouch : MonoBehaviour
{
    [Header("Collider properties")]
    [SerializeField] private ColliderProperties _standingColliderProperties;
    [SerializeField] private ColliderProperties _crouchingColliderProperties;

    private CharacterController _characterController;

    private void Awake()
    {
        this._characterController = this.GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (GameConstants.paused)
            return;
        this.HandleInputs();
    }

    private void HandleInputs()
    {
        if (Input.GetButtonDown(GameConstants.INPUT_CROUCH))
            this.ChangeCrouch();
    }

    private void ChangeCrouch()
    {
        if (!GameConstants.playerVariable.isCrouched)
        {
            GameConstants.playerVariable.isCrouched = true;
            this._characterController.height = this._crouchingColliderProperties.Height;
            this._characterController.center = this._crouchingColliderProperties.Center;
            return;
        }
        else if (Helper.SpaceUp(Helper.GetTop(this.gameObject), this._standingColliderProperties.Height - this._crouchingColliderProperties.Height)) ;
        {
            GameConstants.playerVariable.isCrouched = false;
            this._characterController.height = this._standingColliderProperties.Height;
            this._characterController.center = this._standingColliderProperties.Center;
        }
    }
}
