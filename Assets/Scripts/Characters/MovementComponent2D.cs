using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class MovementComponent2D : MonoBehaviour
{
    public Rigidbody2D _rb { get; private set; }
    public CharacterStats _stats { get; private set; }
    public CapsuleCollider2D _mainCollider { get; private set; }
    public GameObject _spriteObject { get; private set; }

    [Header("Detection Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask thinPlatformLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask stickingWallLayer;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private bool isDefaultFacingRight;

    [Header("Jump Settings")]
    [SerializeField] private int jumpMaxCount;
    [SerializeField] private float jumpHoldTime;


    public bool _isJumpPressed { get; private set; }
    public bool _canFlip { get; private set; }
    public bool _isFacingRight { get; private set; }


    public Vector2 _moveInput { get; private set; }
    public RaycastHit2D _groundHit { get; private set; }
    public RaycastHit2D _wallHit { get; private set; }

    public int _currentJumpCount { get; private set; }
    public float _defaultGravityScale { get; private set; }
    public int JumpMaxCount => jumpMaxCount;
    public float JumpHoldTime => jumpHoldTime;
    

    public GroundedState _groundedState { get; private set; }
    public JumppingState _jumpingState { get; private set; }
    public FallingState _fallingState { get; private set; }
    public DroppingState _droppingState { get; private set; }
    public WallGrabState _wallGrabState { get; private set; }
    public ClimbingState _climbingState { get; private set; }
    public WallStickingState _wallStickingState { get; private set; }

    private MovementStateBase _currentState;

    private IMoveable _moveable;
    private IJumpable _jumpable;
    private IGravityEffect _gravityEffect;

    private void Start()
    {
        CharacterBase character = GetComponent<CharacterBase>();

        _rb = character.rigidBody;
        _mainCollider = character.mainCollider;
        _stats = character.Stats;
        _spriteObject = character.Sprite;

        InitDefaultValue();
        InitStateClass();

        ChangeMoveState(_groundedState);
    }

    private void FixedUpdate()
    {
        _moveable?.Move(_moveInput);
        _gravityEffect?.ApplyGravity();
        _currentState?.OnFixedUpdate();
    }

    private void Update()
    {
        _currentState?.OnUpdate();
    }

    public void ResetJumpCount() => _currentJumpCount = 0;
    public void IncreaseJumpCount() => ++_currentJumpCount;
    public void SetJumpInput(bool pressed) => _isJumpPressed = pressed;
    public void SetCanFlip(bool canFlip) => _canFlip = canFlip;

    public void ChangeMoveState(MovementStateBase newState)
    {
        _currentState?.OnExit();

        if (newState == null) return;

        _currentState = newState;

        _moveable = _currentState as IMoveable;
        _jumpable = _currentState as IJumpable;
        _gravityEffect = _currentState as IGravityEffect;

        _currentState.OnStart();
    }

    public void SetMoveInput(Vector2 moveInput)
    {
        _moveInput = moveInput;

        if (!_canFlip) return;

        if ((moveInput.x < 0 && _isFacingRight) ||
            (moveInput.x > 0 && !_isFacingRight)) Flip(); 
    }

    public bool IsOnThinPlatform()
    {
        if (_currentState != _groundedState) return false;

        return (thinPlatformLayer.value & (1 << _groundHit.collider.gameObject.layer)) != 0;
    }

    public void StartJumppressed()
    {
        if (_rb == null && _stats == null) return;

        _jumpable = _currentState as IJumpable;

        if (_jumpable == null) return;

        SetJumpInput(true);
        _jumpable.Jump();
    }

    public void EndJumppressed()
    {
        SetJumpInput(false);

        //if (_moveState == MovementState.jumping) _moveState = MovementState.falling;
    }

    public void ApplyMovement(float maxSpeed, bool isHorizontal = true, float timeToReach = 1.0f, float timeToStop = 1.0f)
    {
        if (_rb == null || _stats == null) return;

        float inputValue = isHorizontal ? _moveInput.x : _moveInput.y;
        float currentValocity = isHorizontal ? _rb.velocity.x : _rb.velocity.y;
        float targetSpeed = maxSpeed * inputValue;

        float accelUnit = maxSpeed / Mathf.Max(timeToReach, 0.01f);
        float decelUnit = maxSpeed / Mathf.Max(timeToStop, 0.01f);

        float currentRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accelUnit : decelUnit;
        float newValue = Mathf.MoveTowards(currentValocity, targetSpeed, currentRate * Time.fixedDeltaTime);

        Vector2 newVelocity = _rb.velocity;

        if (isHorizontal) newVelocity.x = newValue;
        else newVelocity.y = newValue;

        _rb.velocity = newVelocity;
    }

    public bool CheckGround()
    {
         _groundHit = Physics2D.BoxCast(
            _mainCollider.bounds.center,
            new Vector2(_mainCollider.bounds.size.x * 0.9f, 0.1f),
            0f,
            Vector2.down,
            _mainCollider.bounds.extents.y + groundCheckDistance,
            groundLayer);

        if (_groundHit.collider == null) return false;

        return true;
    }

    public bool CheckWall()
    {
        Vector2 direction = _isFacingRight ? Vector2.right : Vector2.left;

        float rayDistance = wallCheckDistance + 0.05f;
        Vector2 rayStart = (Vector2)_mainCollider.bounds.center + (direction * _mainCollider.bounds.extents.x);

        float xOffset = _mainCollider.bounds.extents.x;
        float yOffset = _mainCollider.bounds.extents.y * 0.8f;

        Vector2 upperOrigin = (Vector2)_mainCollider.bounds.center + new Vector2(direction.x * xOffset, yOffset);
        Vector2 lowerOrigin = (Vector2)_mainCollider.bounds.center + new Vector2(direction.x * xOffset, -yOffset);

        RaycastHit2D upperHit = Physics2D.Raycast(upperOrigin, direction, rayDistance, wallLayer);
        RaycastHit2D lowerHit = Physics2D.Raycast(lowerOrigin, direction, rayDistance, wallLayer);

        _wallHit = upperHit.collider != null ? upperHit : lowerHit;
        //_wallHit = Physics2D.Raycast(rayStart, direction, rayDistance, wallLayer);

        return upperHit.collider != null && lowerHit.collider != null;
        //return _wallHit.collider != null;
    }

    public bool IsPushing()
    {
        bool isPushing = (_isFacingRight && _moveInput.x > 0) || (!_isFacingRight && _moveInput.x < 0);

        return isPushing;
    }

    public bool IsWallGrabable()
    {
        if (!CheckWall()) return false;
        else if (!IsPushing()) return false;

        return true;
    }

    public bool IsStickingWall()
    {
        return (stickingWallLayer.value & (1 << _wallHit.collider.gameObject.layer)) != 0;
    }

    public bool CheckCurrentState(MovementStateBase targetState)
    {
        return _currentState == targetState;
    }

    private void Flip()
    {
        if (_spriteObject == null) return;

        _isFacingRight = !_isFacingRight;

        Vector3 scale = _spriteObject.transform.localScale;
        scale.x *= -1;
        _spriteObject.transform.localScale = scale;
    }

    private void InitDefaultValue()
    {
        _defaultGravityScale = _rb.gravityScale;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        _isJumpPressed = false;
        _moveInput = Vector2.zero;

        _canFlip = true;
        _isFacingRight = isDefaultFacingRight;
    }

    private void InitStateClass()
    {
        _groundedState = new GroundedState(this);
        _jumpingState = new JumppingState(this);
        _fallingState = new FallingState(this);
        _droppingState = new DroppingState(this);
        _wallGrabState = new WallGrabState(this);
        _wallStickingState = new WallStickingState(this);
        _climbingState = new ClimbingState(this);
    }
}