using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementComponent2D : MonoBehaviour
{
    private Rigidbody2D rb;
    private CharacterStats stats;
    private CapsuleCollider2D mainCollider;

    [Header("Detection Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask thinPlatformLayer;
    [SerializeField] private float groundCheckDistance;

    private Vector2 _moveInput;
    private bool _isGrounded;
    private bool _isJumpPressed;
    private float _defaultGravityScale;
    private RaycastHit2D _groundHit;

    public bool IsGrounded => _isGrounded;
    public bool IsJumping => _isJumpPressed;

    public void Init(CapsuleCollider2D newCollider, Rigidbody2D newRb, CharacterStats newStats)
    {
        rb = newRb;
        stats = newStats;
        mainCollider = newCollider;

        _defaultGravityScale = rb.gravityScale;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        _isJumpPressed = false;
        _moveInput = Vector2.zero;
    }

    private void FixedUpdate()
    {
        ApplyHorizontalMovement();
        ApplyGravityModifiers();
    }

    private void Update()
    {
        CheckGround();
    }

    public void SetMoveInput(Vector2 moveInput) => _moveInput = moveInput;
    public void SetJumpInput(bool pressed) => _isJumpPressed = pressed;
    public bool IsOnThinPlatform()
    {
        if (!_isGrounded) return false;

        return (thinPlatformLayer.value & (1 << _groundHit.collider.gameObject.layer)) != 0;
    }

    public void DoJump()
    {
        if (rb == null && stats == null) return;

        rb.velocity = new Vector2(rb.velocity.x, stats.jumpForce);
    }

    public void ActionDropDown()
    {
        if (IsOnThinPlatform())
        {
            StartCoroutine(DisableCollisionRoutine(_groundHit.collider));
        }
    }

    private IEnumerator DisableCollisionRoutine(Collider2D platformCollider)
    {
        Physics2D.IgnoreCollision(mainCollider, platformCollider, true);

        yield return new WaitForSeconds(0.4f);

        Physics2D.IgnoreCollision(mainCollider, platformCollider, false);
    }

    private void ApplyHorizontalMovement()
    {
        if (rb == null || stats == null) return;

        float targetSpeed = _moveInput.x * stats.moveMaxSpeed;

        float timeToReach = _isGrounded ? stats.acceleration_sec : stats.acceleration_air_sec;
        float timeToStop = _isGrounded ? stats.deceleration_sec : stats.deceleration_air_sec;

        float accelUnit = stats.moveMaxSpeed / Mathf.Max(timeToReach, 0.01f);
        float decelUnit = stats.moveMaxSpeed / Mathf.Max(timeToStop, 0.01f);

        float currentRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accelUnit : decelUnit;
        float newX = Mathf.MoveTowards(rb.velocity.x, targetSpeed, currentRate * Time.fixedDeltaTime);

        rb.velocity = new Vector2(newX, rb.velocity.y);
    }

    private void ApplyGravityModifiers()
    {
        if (rb == null && stats == null) return;

        if (rb.velocity.y < 0)
        {
            rb.gravityScale = _defaultGravityScale * stats.fallMultiplier;
        }
        else if (rb.velocity.y > 0 && !_isJumpPressed)
        {
            rb.gravityScale = _defaultGravityScale * stats.lowJumpMultiplier;
        }
        else
        {
            rb.gravityScale = _defaultGravityScale;
        }
    }

    private void CheckGround()
    {
         _groundHit = Physics2D.BoxCast(
            mainCollider.bounds.center,
            new Vector2(mainCollider.bounds.size.x * 0.9f, 0.1f),
            0f,
            Vector2.down,
            mainCollider.bounds.extents.y + groundCheckDistance,
            groundLayer);

        _isGrounded = _groundHit.collider != null;
    }
}
