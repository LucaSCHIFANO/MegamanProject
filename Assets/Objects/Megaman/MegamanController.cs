using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class MegamanController : MonoBehaviour
{
    private Rigidbody2D rb;

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

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        float horizontalMovement = currentJoystickPosition.normalized.x * speed;
        float verticalMovement = rb.velocity.y;

        rb.velocity = new Vector2(horizontalMovement, verticalMovement);
    }

    void Update()
    {
        Jump();
        
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


    #region Inputs


    public void MovementInput(InputAction.CallbackContext context)
    {
        currentJoystickPosition = context.ReadValue<Vector2>();
    }

    public void JumpInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isJumping = true;
            currentJumpTime = jumpTime;
        }
        else if (context.canceled) isJumping = false;
    }

    #endregion
}
