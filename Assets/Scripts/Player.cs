using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Animator animator;

    [SerializeField] private float moveSpeed;
    //[SerializeField] public float runMaxSpeed; //Target speed we want the player to reach.
    //[SerializeField] public float runAcceleration; //The speed at which our player accelerates to max speed, can be set to runMaxSpeed for instant acceleration down to 0 for none at all
    //public float runAccelAmount; //The actual force (multiplied with speedDiff) applied to the player.
    //[SerializeField] public float runDecceleration; //The speed at which our player decelerates from their current speed, can be set to runMaxSpeed for instant deceleration down to 0 for none at all
    //public float runDeccelAmount;

    [SerializeField] private float JumpForce;
    [SerializeField] private float gravityScale;

    [SerializeField] private float coyoteTime;
    [SerializeField] private float coyoteCounter;

    [SerializeField] private float jumpTime;
    [SerializeField] private float jumpFactor = 1f;
    [SerializeField] private int jumpCounter = 1;
    [SerializeField] private bool firstJumpActive;
    [SerializeField] private bool secondJumpActive;
    [SerializeField] private float secondJumpTimer;
    [SerializeField] private float thirdJumpTimer;

    private int coinCount;
    [SerializeField] private Vector3 moveDirection;
    public CharacterController controller;

    public Transform pivot;
    public float rotateSpeed;

    public GameObject playerModel;

    public float knockbackForce;
    public float knockbackTime;
    private float knockbackCounter;

    public PlayerControls playerControls;

    private InputAction move;
    private InputAction jump;
    private InputAction crouch;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();
        jump = playerControls.Player.Jump;
        jump.Enable();
        crouch = playerControls.Player.Crouch;
        crouch.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
        crouch.Disable();
    }

    private void Update()
    {
        if (knockbackCounter <= 0)
        {
            //TODO implement inertia
            float xStore = moveDirection.x;
            float yStore = moveDirection.y;
            float zStore = moveDirection.z;

            moveDirection = move.ReadValue<Vector2>();
            moveDirection = (transform.forward * moveDirection.y) + (transform.right * moveDirection.x);
            moveDirection = moveDirection.normalized * moveSpeed;
            
            moveDirection.y = yStore;

            //if on the ground, reset y movement and coyote counter
            if (controller.isGrounded)
            {
                moveDirection.y = 0f;
                coyoteCounter = coyoteTime;
            }
            else
            {
                coyoteCounter -= Time.deltaTime;
            }

            //check if Jump is pressed
            if (jump.triggered && coyoteCounter > 0f)
            {
                //Backflip
                if (crouch.IsPressed())
                {
                    jumpFactor = 1.4f;
                }
                else
                {
                    if (jumpCounter == 1)
                    {
                        jumpFactor = 1f;
                        jumpCounter++;
                        firstJumpActive = true;
                    }
                    else if (jumpCounter == 2 && firstJumpActive && secondJumpTimer > 0f)
                    {
                        jumpFactor = 1f;
                        jumpCounter++;
                        secondJumpTimer = 0f;
                        firstJumpActive = false;
                        secondJumpActive = true;
                    }
                    else if (jumpCounter == 3 && secondJumpActive && thirdJumpTimer > 0f)
                    {
                        jumpFactor = 1.3f;
                        jumpCounter++;
                        thirdJumpTimer = 0f;
                        firstJumpActive = false;
                        secondJumpActive = false;
                    }
                }
                moveDirection.y = JumpForce * jumpFactor;
                coyoteCounter = 0f;
            }
            if (jumpCounter > 3)
            {
                jumpCounter = 1;
            }
            if (secondJumpTimer >= jumpTime)
            {
                jumpFactor = 1f;
                jumpCounter = 1;
                secondJumpTimer = 0f;
                firstJumpActive = false;
            }

            if (thirdJumpTimer >= jumpTime)
            {
                jumpFactor = 1f;
                jumpCounter = 1;
                thirdJumpTimer = 0f;
                secondJumpActive = false;
            }
            if (firstJumpActive && coyoteCounter > 0f)
            {
                secondJumpTimer += Time.deltaTime;
            }

            if (secondJumpActive && coyoteCounter > 0f)
            {
                thirdJumpTimer += Time.deltaTime;
            }

            if(crouch.WasReleasedThisFrame())
            {
                jumpFactor = 1f;
            }

            //if Jump is let go then start falling
            if (jump.WasReleasedThisFrame() && moveDirection.y > 0)
            {
                moveDirection.y = -0.2f;
            }
        }
        else
        {
            knockbackCounter -= Time.deltaTime;
        }

        moveDirection.y += (Physics.gravity.y * (gravityScale - 1) * Time.deltaTime);
        controller.Move(moveDirection * Time.deltaTime);

        //Move player direction
        if (moveDirection.x != 0 || moveDirection.z != 0)
        {
            transform.rotation = Quaternion.Euler(0f, pivot.rotation.eulerAngles.y, 0f);
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rotateSpeed * Time.deltaTime);
        }

        animator.SetBool("IsGrounded", controller.isGrounded);
        animator.SetBool("IsRunning", (Mathf.Abs(moveDirection.z) + Mathf.Abs(moveDirection.x)) > 0);
        //TODO: animator.SetBool("IsCrouched")
    }
    public void Knockback(Vector3 direction)
    {
        knockbackCounter = knockbackTime;

        moveDirection = direction * knockbackForce;
        moveDirection.y = knockbackForce * 0.4f;
    }
}
