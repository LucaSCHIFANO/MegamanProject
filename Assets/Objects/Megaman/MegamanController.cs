using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class MegamanController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private BoxCollider2D bc;
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Movement")]
    [SerializeField] private float speed;
    private Vector2 currentJoystickPosition;

    [Header("Jump")]
    [SerializeField] private float jumpForce;
    private bool isJumping;
    [SerializeField] private float jumpTime;
    private float currentJumpTime;

    [SerializeField] private float gravityJump;
    [SerializeField] private float gravityFall;

    [SerializeField] float extraHeightBelow;
    [SerializeField] private LayerMask ground;
    [SerializeField] private bool DebugGroundCheck;


    [Header("Animation")]
    private bool isRunningAnim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Movement();   
    }

    void Update()
    {
        Jump();

        UpdateAnimation();
    }

    private void Movement()
    {
        float horizontalMovement = currentJoystickPosition.normalized.x * speed;
        float verticalMovement = rb.velocity.y;

        rb.velocity = new Vector2(horizontalMovement, verticalMovement);
    }


    private void Jump()
    {
        if (isJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            currentJumpTime -= Time.deltaTime;
        }

        if (currentJumpTime < 0) isJumping = false;

        if (isJumping) rb.gravityScale = gravityJump;
        else rb.gravityScale = gravityFall;
    }

    public bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0f, Vector2.down, extraHeightBelow, ground);

        if (raycastHit.collider != null)
        {
            return raycastHit.collider != null;
        }

        return false;
    }

    #region Debug

    private void OnDrawGizmosSelected()
    {
        if (DebugGroundCheck && bc != null)
        {
            Debug.DrawRay(bc.bounds.center + new Vector3(bc.bounds.extents.x, -bc.bounds.extents.y), Vector2.down * (extraHeightBelow), Color.red);
            Debug.DrawRay(bc.bounds.center - new Vector3(bc.bounds.extents.x, bc.bounds.extents.y), Vector2.down * (extraHeightBelow), Color.red);
            Debug.DrawRay(bc.bounds.center - new Vector3(bc.bounds.extents.x, bc.bounds.extents.y + extraHeightBelow), Vector2.right * (bc.bounds.extents.x) * 2, Color.red);
        }
    }


    #endregion


    #region Animation

    private void UpdateAnimation()
    {
        if (currentJoystickPosition.x < 0f) sr.flipX = false;
        else if (currentJoystickPosition.x > 0f) sr.flipX = true;

        if (IsGrounded())
        {
            if (Mathf.Abs(rb.velocity.x) > 0f)
            {
                if (!isRunningAnim)
                {
                    isRunningAnim = true;
                    animator.Play("Megaman_PreRun");
                }
            }
            else
            {
                animator.Play("Megaman_Idle");
                isRunningAnim = false;
            }
        }
        else
        {
            isRunningAnim = false;
            animator.Play("Megaman_Jump");
        }
        
    }

    #endregion


    #region Inputs


    public void MovementInput(InputAction.CallbackContext context)
    {
        currentJoystickPosition = context.ReadValue<Vector2>();
    }

    public void JumpInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!IsGrounded()) return;
            isJumping = true;
            currentJumpTime = jumpTime;
        }
        else if (context.canceled) isJumping = false;
    }

    #endregion
}
