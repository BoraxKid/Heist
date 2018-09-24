using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Player variable")]
    [SerializeField] private PlayerVariable _variable;

    [Header("Properties")]
    [SerializeField] private FloatReference _gravity;
    [SerializeField] private FloatReference _airControl;
    [SerializeField] private FloatReference _maxGroundedDistance;
    [SerializeField] private LayerMask _groundMask;

    [Header("Requirements")]
    [SerializeField] private Transform _cameraTransform;

    private CharacterController _characterController;
    private Vector3 _cameraForward;
    private Vector3 _movement;
    private Vector2 _movementInput;

    private void Awake()
    {
        if (this._cameraTransform == null)
        {
            if (Camera.main)
                this._cameraTransform = Camera.main.transform;
            else
                Debug.LogError("No main camera");
        }
        this._characterController = this.GetComponent<CharacterController>();

        this._variable.gameObject = this.gameObject;
        this._variable.collider = this.GetComponentInChildren<Collider>();
        this._variable.isAlive = true;
        this._variable.isGrounded = true;
        this._variable.isCrouched = false;
        this._variable.movement = Vector3.zero;
        this._variable.velocity = Vector3.zero;
        this._variable.brightness = 0.0f;
        GameConstants.playerVariable = this._variable;
    }

    private void Update()
    {
        if (GameConstants.paused)
            return;
        this.HandleInputs();
    }

    private void FixedUpdate()
    {
        if (GameConstants.paused)
            return;
        this.ComputeCameraRelativeMovement();
        this.CharacterControllerMovement();
        this.UpdateVariable();
    }

    private void HandleInputs()
    {
        this._movementInput.x = Input.GetAxis(GameConstants.INPUT_AXIS_X);
        this._movementInput.y = Input.GetAxis(GameConstants.INPUT_AXIS_Y);
    }

    private void ComputeCameraRelativeMovement()
    {
        if (!this._characterController.isGrounded)
        {
            this._movementInput.x *= this._airControl.Value;
            this._movementInput.y *= this._airControl.Value;
        }
        if (this._cameraTransform)
        {
            this._cameraForward = Vector3.Scale(this._cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
            this._movement = this._movementInput.y * this._cameraForward + this._movementInput.x * this._cameraTransform.right;
        }
        else
        {
            this._movement = this._movementInput.y * Vector3.forward + this._movementInput.x * Vector3.right;
        }
        if (this._movement.magnitude > 1) // Make sure we don't go faster when going diagonally
            this._movement.Normalize();
        if (this._variable.isCrouched)
            this._movement *= this._variable.crouchSpeed;
        else
            this._movement *= this._variable.normalSpeed;
        this._movement.y -= this._gravity.Value * Time.fixedDeltaTime;
    }

    private void CharacterControllerMovement()
    {
        this._characterController.Move(this._movement * Time.fixedDeltaTime);
        Vector3 tmp = this._movement;
        tmp.y = 0;
        if (tmp == Vector3.zero)
            return;
        this.transform.rotation = Quaternion.LookRotation(tmp, Vector3.up);
    }

    private void UpdateVariable()
    {
        this._variable.movement = this._movement;
        this._variable.movementInput = this._movementInput;
        this._variable.velocity = this._characterController.velocity;
        this.ChechGroundDistance();
    }

    private void ChechGroundDistance()
    {
        Vector3 bottomPos = Helper.GetBottom(this.gameObject);

        if (Physics.Raycast(bottomPos, -this.transform.up, this._maxGroundedDistance.Value, this._groundMask))
            this._variable.isGrounded = true;
        else
            this._variable.isGrounded = false;
    }
}
