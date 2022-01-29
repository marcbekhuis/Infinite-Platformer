using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [Header("Velocity")]
    [SerializeField] [Min(0)] private float groundSpeed = 2;
    [SerializeField] [Min(0)] private float airSpeed = 1;
    [SerializeField] [Min(0)] private float swimSpeed = 1;
    [SerializeField] [Min(0)] private float airDrag = 1;
    [Header("Jump")]
    [SerializeField] [Min(0)] private float jumpHeight = 2;
    [SerializeField] [Min(0)] private float jumpCooldown = 1;
    [SerializeField] [Min(0)] private float airJumpHeight = 2;
    [SerializeField] [Min(0)] private float airJumpCooldown = 1;
    [SerializeField] [Min(0)] private int airJumps = 3;
    [Header("Air check")]
    [SerializeField] private Vector3 airCheckOffset = Vector3.down;
    [SerializeField] private Vector3 airCheckHalfSize = Vector3.one;
    [SerializeField] private LayerMask groundLayers;
    [Header("Save ground Check")]
    [SerializeField] private Vector3 saveGroundCheckOffsetRight = Vector3.right;
    [SerializeField] private Vector3 saveGroundCheckOffsetLeft = Vector3.left;
    [SerializeField] private Vector3 saveGroundCheckHalfSize = Vector3.one;
    [Header("Camera")]
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, -10, 0);

    private new Rigidbody rigidbody;
    private float jumpTimer = 0;
    private float airJumpTimer = 0;
    private bool inAir = false;
    private Vector3 lastSaveGroundPosition;
    private int airJumpsUsed = 0;
    private bool jumpedInWater = false;

    private bool inWater = false;

    public enum State 
    {
        OnGround,
        InAir,
        InWater
    }

    public bool InWater
    {
        get => inWater;
        set
        {
            inWater = value;
            jumpedInWater = false;
        }
    }

    public State state
    {
        get
        {
            if (inWater)
                return State.InWater;
            if (inAir)
                return State.InAir;
            return State.OnGround;
        }
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        CameraController.instance.SetDestination(transform.position + cameraOffset);
    }

    private void Update()
    {
        if (Gamemanager.instance.PlayerMovementBlocked)
            return;
        AirCheck();
        Jump();
        AirJump();
        MoveForwardBackward();
        CheckForSaveGround();
        CameraController.instance.SetDestination(transform.position + cameraOffset + rigidbody.velocity.normalized * 2);
    }

    private void Jump()
    {
        jumpTimer -= Time.deltaTime;

        if (state == State.InAir)
            return;
        if (jumpedInWater)
            return;
        if (jumpTimer > 0)
            return;

        if (Gamemanager.instance.input.Jump())
        {
            rigidbody.AddForce(new Vector3(0, jumpHeight, 0), ForceMode.Impulse);
            jumpTimer += jumpCooldown;

            if (state == State.InWater)
                jumpedInWater = true;
        }
    }

    private void AirJump()
    {
        airJumpTimer -= Time.deltaTime;

        if (state != State.InAir)
            return;
        if (airJumpTimer > 0)
            return;
        if (airJumpsUsed >= airJumps)
            return;

        if (Gamemanager.instance.input.Jump())
        {
            airJumpsUsed++;
            rigidbody.AddForce(new Vector3(0, airJumpHeight + Mathf.Clamp(rigidbody.velocity.y * -1, 0 , 10), 0), ForceMode.Impulse);
            airJumpTimer += airJumpCooldown;
        }
    }

    private void MoveForwardBackward()
    {
        float velocity = 0;
        switch (state)
        {
            case State.OnGround:
                velocity += groundSpeed * Gamemanager.instance.input.MoveRight();
                velocity += groundSpeed * Gamemanager.instance.input.MoveLeft();
                rigidbody.velocity = new Vector3(velocity, rigidbody.velocity.y, 0);
                break;
            case State.InAir:
                velocity += airSpeed * Gamemanager.instance.input.MoveRight();
                velocity += airSpeed * Gamemanager.instance.input.MoveLeft();
                rigidbody.AddForce(new Vector3(velocity * Time.deltaTime, 0, 0), ForceMode.Force);
                if (rigidbody.velocity.x > 0)
                    rigidbody.velocity = new Vector3(Mathf.Clamp(rigidbody.velocity.x - airDrag * Time.deltaTime, 0, float.MaxValue), rigidbody.velocity.y, 0);
                if (rigidbody.velocity.x < 0)
                    rigidbody.velocity = new Vector3(Mathf.Clamp(rigidbody.velocity.x + airDrag * Time.deltaTime, float.MinValue, 0), rigidbody.velocity.y, 0);
                break;
            case State.InWater:
                velocity += swimSpeed * Gamemanager.instance.input.MoveRight();
                velocity += swimSpeed * Gamemanager.instance.input.MoveLeft();
                rigidbody.velocity = new Vector3(velocity, rigidbody.velocity.y, 0);
                break;
            default:
                break;
        }
    }

    private void AirCheck()
    {
        inAir = !Physics.CheckBox(transform.position + transform.rotation * airCheckOffset, airCheckHalfSize, transform.rotation, groundLayers);
        if (!inAir)
            airJumpsUsed = 0;
    }

    private void CheckForSaveGround()
    {
        if (Physics.CheckBox(transform.position + transform.rotation * saveGroundCheckOffsetLeft, saveGroundCheckHalfSize, transform.rotation, groundLayers)
        && Physics.CheckBox(transform.position + transform.rotation * saveGroundCheckOffsetRight, saveGroundCheckHalfSize, transform.rotation, groundLayers))
            lastSaveGroundPosition = transform.position;
    }

    public void Die()
    {
        rigidbody.velocity = Vector3.zero;
        transform.position = lastSaveGroundPosition + new Vector3(0, 0.5f, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + transform.rotation * airCheckOffset, airCheckHalfSize * 2);
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(transform.position + transform.rotation * saveGroundCheckOffsetRight, saveGroundCheckHalfSize * 2);
        Gizmos.DrawCube(transform.position + transform.rotation * saveGroundCheckOffsetLeft, saveGroundCheckHalfSize * 2);
    }
}
