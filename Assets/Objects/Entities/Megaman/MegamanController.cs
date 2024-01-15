using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MegamanController : Entity, IBulletEmiter
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private BoxCollider2D slideBoxCollider;
    private BoxCollider2D currentBoxCollider;

    [SerializeField] float defaulContactOffsetMultiplicator;
    private float defaultContactOffset;
    private Rigidbody2D rb;
    private Animator animator;


    [Header("Movement")]
    [SerializeField] private float speed;
    private Vector2 currentJoystickPosition;
    private bool isCurrentlyGrounded;
    private MegamanState state;

    private bool joystickReset = true;
    [SerializeField] private float preRunTime;
    private float currentPreRunTime;


    [Header("RoomTransition")]
    private Vector2 roomTransitionTarget;
    private float transitionSpeed;


    [Header("Jump")]
    [SerializeField] private float jumpForce;
    private bool isJumping;
    [SerializeField] private float jumpTime;
    private float currentJumpTime;

    [SerializeField] private float gravityJump;
    [SerializeField] private float gravityFall;

    [SerializeField] float extraHeightBelow;
    [SerializeField] float extraHeightAbove;
    [SerializeField] private LayerMask ground;
    [SerializeField] private bool DebugDebugCollisionCheckCheck;


    [Header("Shoot")]
    [SerializeField] private Bullet bullet;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Transform jumpingShootPoint;

    [SerializeField] private float shootAnimDuration;
    private float currentShootAnimDuration;

    private PoolBulletManager poolBulletManager;
    private List<Bullet> bulletList = new List<Bullet>();


    [Header("Slide")]
    [SerializeField] private float slideSpeed;
    private bool isSliding;

    [SerializeField] private float slideTime;
    private float currentSlideTime;

    private bool isSlideRight;


    [Header("Ladder")]
    private Ladder closeLadder;
    private bool isCloseToLadder;
    private bool isClimbing;
    private float ladderBoundsOffset;
    private float ladderExitPositionOffset;



    [Header("Animation")]
    private bool isRunningAnim;
    private bool isShootingAnim;
    private bool lastShootingAnim;

    enum MegamanState
    {
        CanMove,
        MovementLock,
        RoomTransition,
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        SetCurrentCollider(false);
        defaultContactOffset = Physics2D.defaultContactOffset *2;

        ladderBoundsOffset = currentBoxCollider.size.y / 3f;
        ladderExitPositionOffset = currentBoxCollider.size.y / 2f;
    }

    void Start()
    {
        poolBulletManager = PoolBulletManager.Instance;
    }


    private void FixedUpdate()
    {
        if (state == MegamanState.CanMove) 
        {
            Climb();
            Movement();
        }
    }

    void Update()
    {
        isCurrentlyGrounded = IsGrounded();

        switch (state)
        {
            case MegamanState.CanMove:
                Jump(); Slide(); UpdateAnimation();
                break;

            case MegamanState.MovementLock:
                break;

            case MegamanState.RoomTransition:
                MoveTo();
                break;
        }

        if (IsTouchingRoof()) isJumping = false;
        
    }


    #region Collisions 

    public bool IsGrounded()
    {
       // if(currentBoxCollider == null) return false;

        RaycastHit2D raycastHit = Physics2D.BoxCast(currentBoxCollider.bounds.center,
            currentBoxCollider.bounds.size + new Vector3(defaultContactOffset * defaulContactOffsetMultiplicator, 0, 0), 0f,
            Vector2.down, extraHeightBelow, ground);

        if (raycastHit.collider != null)
        {
            return raycastHit.collider != null;
        }

        return false;
    }

    public bool IsTouchingRoof()
    {
       // if (currentBoxCollider == null) return false;

        RaycastHit2D raycastHit = Physics2D.BoxCast(currentBoxCollider.bounds.center,
            currentBoxCollider.bounds.size, 0f,
            Vector2.up, extraHeightAbove, ground);

        if (raycastHit.collider != null)
        {
            return raycastHit.collider != null;
        }

        return false;
    }

    private void SetCurrentCollider(bool isSliding)
    {
        if (isSliding)
        {
            boxCollider.enabled = false;
            slideBoxCollider.enabled = true;
            currentBoxCollider = slideBoxCollider;
        }
        else{
            boxCollider.enabled = true;
            slideBoxCollider.enabled = false;
            currentBoxCollider = boxCollider;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("RoomTransition"))
        {
            StartCoroutine(RoomTransition(collision.GetComponent<RoomTransition>()));
        }
        else if (collision.CompareTag("Ladder"))
        {
            isCloseToLadder = true;
            closeLadder = collision.gameObject.GetComponent<Ladder>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isCloseToLadder = false;
            closeLadder = null;
        }
    }


    #endregion


    #region Movement

    private void ChangeState(MegamanState _state)
    {
        state = _state;

        switch (state)
        {
            case MegamanState.CanMove:
                if(!isClimbing) rb.gravityScale = gravityFall;
                if (currentJoystickPosition == Vector2.zero) joystickReset = true;
                break;

            case MegamanState.MovementLock:
                break;

            case MegamanState.RoomTransition:
                isJumping = false;
                rb.velocity = Vector2.zero;
                rb.gravityScale = 0;
                break;
        }

    }

    private void Climb()
    {
        float joystickY = currentJoystickPosition.y;
        if (joystickY != 0) joystickY = Mathf.Sign(joystickY);


        float verticalMovement = joystickY * speed;


        if (isClimbing)
        {
            rb.velocity = new Vector2(0, verticalMovement);

            if (joystickY > 0 && transform.position.y > closeLadder.TopHandler.position.y + ladderBoundsOffset)
            {
                transform.position = closeLadder.TopHandler.position + new Vector3(0, ladderExitPositionOffset, 0);
            }
            else if(transform.position.y < closeLadder.BotHandler.transform.position.y - ladderBoundsOffset) 
            {
                transform.position = closeLadder.BotHandler.position + new Vector3(0, -ladderExitPositionOffset, 0);
            }
            else if (joystickY >= 0 || !IsGrounded() || (transform.position.y >= closeLadder.transform.position.y))
            {
                return;
            }

            rb.velocity = Vector2.zero;
            StopClimbing();
        }
        else
        {
            if (joystickY != 0 && isCloseToLadder)
            {
                if (joystickY > 0 && transform.position.y > closeLadder.TopHandler.position.y ||
                    (joystickY < 0 && IsGrounded() && (transform.position.y < closeLadder.TopHandler.position.y))) return;

                isClimbing = true;
                rb.gravityScale = 0f;
                rb.velocity = Vector2.zero;

                rb.bodyType = RigidbodyType2D.Kinematic;
                var positionOnLadder = transform.position.y;
                if (transform.position.y > closeLadder.TopHandler.position.y) positionOnLadder -= ladderExitPositionOffset;
                else if (transform.position.y < closeLadder.BotHandler.position.y) positionOnLadder += ladderExitPositionOffset;
                transform.position = new Vector3(closeLadder.transform.position.x, positionOnLadder, 0);
              
            
            }
        }
    }

    private void StopClimbing()
    {
        isClimbing = false;
        rb.gravityScale = gravityFall;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void Movement()
    {
        if (isSliding || isClimbing) return;

        float joystickX = currentJoystickPosition.x;
        if (joystickX != 0) joystickX = Mathf.Sign(joystickX);

        float horizontalMovement = joystickX * speed;
        float verticalMovement = rb.velocity.y;


        if (joystickReset && joystickX != 0)
        {
            joystickReset = false;
            currentPreRunTime = preRunTime;
            rb.velocity = new Vector2(horizontalMovement, verticalMovement);
        }
        else if (currentPreRunTime <= 0)
        {
            rb.velocity = new Vector2(horizontalMovement, verticalMovement);

        }
        else
        {
            rb.velocity = new Vector2(0, verticalMovement);
            currentPreRunTime -= Time.deltaTime;
        }
            
    }

    private void Jump()
    {
        if(isClimbing) return;

        if (isJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            currentJumpTime -= Time.deltaTime;
            rb.gravityScale = gravityJump;
        }
        else rb.gravityScale = gravityFall;


        if (currentJumpTime < 0) isJumping = false;

    }

    private void Shoot()
    {
        if(isSliding || isClimbing) return;

        var bulletSpawnPoint = transform.position;
        var bulletDirection = Vector2.left;
        var currentShootPoint = Vector3.zero;

        if (isCurrentlyGrounded)
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

    private void Slide()
    {
        if (!isCurrentlyGrounded || (sr.flipX != isSlideRight && !IsTouchingRoof()))
        {
            isSliding = false;
            SetCurrentCollider(false);
        }
        if (isSliding)
        {
            var currentSlideSpeed = -slideSpeed;
            if (sr.flipX) currentSlideSpeed *= -1;


            rb.velocity = new Vector2(currentSlideSpeed, rb.velocity.y);
            currentSlideTime -= Time.deltaTime;
        }


        if (currentSlideTime < 0 && !IsTouchingRoof())
        {
            SetCurrentCollider(false);
            isSliding = false;
        }

    }





    #endregion

    
    #region Room Transition

    private void MoveTo()
    {
        transform.position = Vector2.MoveTowards(transform.position, roomTransitionTarget, transitionSpeed * Time.deltaTime);
    }

    IEnumerator RoomTransition(RoomTransition roomTransition)
    {
        ChangeState(MegamanState.RoomTransition);
        switch (roomTransition.TransitionSide)
        {
            case Room.TransitionSide.Left:
                roomTransitionTarget = transform.position + new Vector3(-GameData.roomColliderThickness * GameData.roomTransitionDistance, 0, 0); 
                break;
            case Room.TransitionSide.Right:
                roomTransitionTarget = transform.position + new Vector3(GameData.roomColliderThickness * GameData.roomTransitionDistance, 0, 0);
                break;
            case Room.TransitionSide.Bottom:
                roomTransitionTarget = transform.position + new Vector3(0, -GameData.roomColliderThickness * GameData.roomTransitionDistance, 0);
                break;
            case Room.TransitionSide.Top:
                roomTransitionTarget = transform.position + new Vector3(0, GameData.roomColliderThickness * GameData.roomTransitionDistance, 0);
                break;
            default:
                break;
        }
        transitionSpeed = Vector2.Distance(transform.position, roomTransitionTarget) / GameData.roomTransitionTime;

        yield return new WaitForSeconds(GameData.roomTransitionTime);

        ChangeState(MegamanState.CanMove);
    }


    #endregion


    #region Debug

    private void OnDrawGizmosSelected()
    {
        if (DebugDebugCollisionCheckCheck && currentBoxCollider != null)
        {
            Debug.DrawRay(currentBoxCollider.bounds.center + new Vector3(currentBoxCollider.bounds.extents.x + defaultContactOffset * defaulContactOffsetMultiplicator, -currentBoxCollider.bounds.extents.y), Vector2.down * (extraHeightBelow), Color.red);
            Debug.DrawRay(currentBoxCollider.bounds.center - new Vector3(currentBoxCollider.bounds.extents.x + defaultContactOffset * defaulContactOffsetMultiplicator, currentBoxCollider.bounds.extents.y), Vector2.down * (extraHeightBelow), Color.red);
            Debug.DrawRay(currentBoxCollider.bounds.center - new Vector3(currentBoxCollider.bounds.extents.x + defaultContactOffset * defaulContactOffsetMultiplicator, currentBoxCollider.bounds.extents.y + extraHeightBelow), Vector2.right * (currentBoxCollider.bounds.extents.x + defaultContactOffset * defaulContactOffsetMultiplicator) * 2, Color.red);

            Debug.DrawRay(currentBoxCollider.bounds.center + new Vector3(currentBoxCollider.bounds.extents.x, currentBoxCollider.bounds.extents.y), Vector2.up * (extraHeightAbove), Color.red);
            Debug.DrawRay(currentBoxCollider.bounds.center + new Vector3(-currentBoxCollider.bounds.extents.x, currentBoxCollider.bounds.extents.y), Vector2.up * (extraHeightAbove), Color.red);
            Debug.DrawRay(currentBoxCollider.bounds.center + new Vector3(-currentBoxCollider.bounds.extents.x, currentBoxCollider.bounds.extents.y + extraHeightAbove), Vector2.right * (currentBoxCollider.bounds.extents.x) * 2, Color.red);

        }
    }


    #endregion


    #region Animation

    private void UpdateAnimation()
    {
        string animName = "";
        if (isClimbing)
        {
            if (transform.position.y > closeLadder.TopHandler.position.y) animName = "Megaman_Climb_TopLadder";
            else animName = "Megaman_Climb";
        }
        else
        {
            if (currentJoystickPosition.x < 0f) sr.flipX = false;
            else if (currentJoystickPosition.x > 0f) sr.flipX = true;

            if (isCurrentlyGrounded)
            {
                if (currentPreRunTime > 0)
                {
                    animName = "Megaman_PreRun";
                }
                else if (Mathf.Abs(rb.velocity.x) > 0.1f)
                {
                    if (isSliding)
                    {
                        isRunningAnim = false;
                        animName = "Megaman_Slide";
                    }
                    else if (currentPreRunTime <= 0f) animName = "Megaman_Run";
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
        }

        if (animName != "")
        {
            if (lastShootingAnim != isShootingAnim)
            {
                lastShootingAnim = isShootingAnim;
                animator.Play(animName, 0, animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1);
            }
            else
            {
                if(isClimbing && Mathf.Abs(rb.velocity.y) == 0) animator.speed = 0;
                else animator.speed = 1;
                animator.Play(animName);
            }
        }
    }

    #endregion


    #region Inputs


    public void MovementInput(InputAction.CallbackContext context)
    {
        currentJoystickPosition = context.ReadValue<Vector2>();

        if (currentJoystickPosition.x == 0 && state == MegamanState.CanMove && isCurrentlyGrounded) joystickReset = true;
    }

    public void JumpInput(InputAction.CallbackContext context)
    {
        if (state != MegamanState.CanMove) return;

        if (context.performed)
        {
            if(isClimbing) StopClimbing();
            if (!isCurrentlyGrounded || IsTouchingRoof()) return;

            joystickReset = false;
            currentPreRunTime = 0;

            if (currentJoystickPosition.y < 0 && !isSliding) 
            {
                isSliding = true;
                currentSlideTime = slideTime;
                isSlideRight = sr.flipX;
                SetCurrentCollider(true);
            }
            else
            {
                isJumping = true;
                currentJumpTime = jumpTime;

                isSliding = false;
            }
        }
        else if (context.canceled) isJumping = false;
    }

    public void ShootInput(InputAction.CallbackContext context)
    {
        if (state != MegamanState.CanMove) return;

        if (context.performed)
        {
            Shoot();
        }
    }

    #endregion
}
