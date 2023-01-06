using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [Header("Velocity")]
    [SerializeField] [Min(0)] private float groundSpeed = 2;
    [SerializeField] [Min(0)] private float swimSpeed = 1;
    [SerializeField] [Min(0)] private float airSpeed = 1;
    [SerializeField] [Min(0)] private float airDrag = 1;
    [SerializeField] [Min(0)] private float airGain = 1;
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
    [SerializeField] private Vector3 spawnPosition = new Vector3(0, 0.5f, 0);
    [SerializeField] private float spawnHeightCheckOffset = 3;
    [SerializeField] private Vector3 cameraSpawnOffset = new Vector3(0, 2.1f, -10);

    public UnityEvent onDeath = new UnityEvent();

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
    }

    private void Start()
    {
        onDeath.AddListener(GameManager.instance.GameOver);
        Respawn();
    }

    public void Respawn()
    {
        RaycastHit[] hits = Physics.RaycastAll(spawnPosition + Vector3.up * spawnHeightCheckOffset, Vector3.down, spawnHeightCheckOffset * 2, groundLayers);
        Vector3 position = spawnPosition;
        if(hits.Length > 0)
        {
            position = hits[0].point;
        }

        this.transform.position = position;
        Camera.main.transform.position = cameraSpawnOffset;
    }

    public void UpdateLogic()
    {
        if (GameManager.instance.PlayerMovementBlocked)
        {
            rigidbody.Sleep();
            return;
        }
        AirCheck();
        Jump();
        AirJump();
        MoveForwardBackward();
        CheckForSaveGround();
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

        if (InputManager.instance.Jump())
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

        if (InputManager.instance.Jump())
        {
            airJumpsUsed++;
            rigidbody.AddForce(new Vector3(0, airJumpHeight + Mathf.Clamp(rigidbody.velocity.y * -1, 0 , 10), 0), ForceMode.Impulse);
            airJumpTimer += airJumpCooldown;
        }
    }

    private void MoveForwardBackward()
    {
        float velocity = 0;
        int direction = InputManager.instance.MoveRight() + InputManager.instance.MoveLeft();
        switch (state)
        {
            case State.OnGround:
                velocity = groundSpeed * direction;        
                break;
            case State.InAir:
                if(direction == 0)
                {
                    velocity = rigidbody.velocity.x - Mathf.Clamp(rigidbody.velocity.x, -1, 1) * airDrag * Time.deltaTime;
                }
                else
                {
                    velocity = Mathf.Clamp(rigidbody.velocity.x + airGain * Time.deltaTime * direction, -airSpeed, airSpeed);
                }
                break;
            case State.InWater:
                velocity = swimSpeed * direction;
                break;
            default:
                break;
        }
        rigidbody.velocity = new Vector3(velocity, rigidbody.velocity.y, 0);
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
        onDeath.Invoke();
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
