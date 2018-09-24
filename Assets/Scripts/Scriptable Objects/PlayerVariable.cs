using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Variables/Player", fileName = "New player variable")]
public class PlayerVariable : ScriptableObject
{
    [Header("Constants")]
    [SerializeField] private FloatReference _normalSpeed;
    [SerializeField] private FloatReference _crouchSpeed;

    public float normalSpeed { get { return (this._normalSpeed.Value); } }
    public float crouchSpeed { get { return (this._crouchSpeed.Value); } }

    [Header("Variables")]
    public GameObject gameObject;
    public Collider collider;
    public Vector3 movement;
    public Vector2 movementInput;
    public Vector3 velocity;
    public bool isAlive { get; set; }
    public bool isGrounded;
    public bool isCrouched;
    public float brightness;
}
