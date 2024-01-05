using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class MegamanController : Entity, IBulletEmiter
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

    [Header("Shoot")]
    [SerializeField] private Bullet bullet;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Transform jumpingShootPoint;

    [SerializeField] private float shootAnimDuration;
    private float currentShootAnimDuration;

    private PoolBulletManager poolBulletManager;
    private List<Bullet> bulletList = new List<Bullet>();


    [Header("Animation")]
    private bool isRunningAnim;
    private bool isShootingAnim;
    private bool lastShootingAnim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        poolBulletManager = PoolBulletManager.Instance;
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
        float joystickX = currentJoystickPosition.x;
        if (joystickX != 0) joystickX = Mathf.Sign(joystickX);

        float horizontalMovement = joystickX * speed;
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

    private void Shoot()
    {
        var bulletSpawnPoint = transform.position;
        var bulletDirection = Vector2.left;
        var currentShootPoint = Vector3.zero;

        if (IsGrounded())
        {
            bulletSpawnPoint += shootPoint.localPosition;
            currentShootPoint = shootPoint.localPosition;

        }
        else
        {
            bulletSpawnPoint += jumpingShootPoint.localPosition;
            currentShootPoint = jumpingShootPoint.localPosition;
        }

        

        if (sr.flipX)
        {
            bulletDirection = Vector2.right;
            bulletSpawnPoint = new Vector3(bulletSpawnPoint.x + Mathf.Abs(currentShootPoint.x * 2), bulletSpawnPoint.y);
        }

        if (bulletList.Count < 3)
        {
            var newBullet = poolBulletManager.Pool.Get();
            newBullet.Init(bulletSpawnPoint, bulletDirection, poolBulletManager.Pool.Release, this);
            bulletList.Add(newBullet);

            currentShootAnimDuration = shootAnimDuration;
        }
    }

    public void BulletDestroyed(Bullet bullet)
    {
        bulletList.Remove(bullet);
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

        string animName = "";

        if (IsGrounded())
        {
            if (Mathf.Abs(rb.velocity.x) > 0f)
            {
                if (!isRunningAnim)
                {
                    isRunningAnim = true;
                    animName = "Megaman_PreRun";
                }else animName = "Megaman_Run";
            }
            else
            {
                animName = "Megaman_Idle";
                isRunningAnim = false;
            }
        }
        else
        {
            isRunningAnim = false;
            animName = "Megaman_Jump";
        }

        if (currentShootAnimDuration > 0f)
        {
            animName += "_Shoot";
            currentShootAnimDuration -= Time.deltaTime;
            isShootingAnim = true;
        }
        else isShootingAnim = false;

        if (animName != "")
        {
            if (lastShootingAnim != isShootingAnim)
            {
                lastShootingAnim = isShootingAnim;
                animator.Play(animName, 0, animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1);
            }
            else
            {
                animator.Play(animName);
            }
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

    public void ShootInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Shoot();
        }
    }

    #endregion
}
