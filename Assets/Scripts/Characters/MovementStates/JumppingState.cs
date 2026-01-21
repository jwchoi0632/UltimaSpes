using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumppingState : MovementStateBase, IMoveable, IJumpable, IGravityEffect
{
    private float _currentJumpTime;
    private float _grabHoldTime = 0.0f;
    private float _grabBlockTime = 0.3f;
    private float _currentblockTime;
    private float _currentHoldTime;

    public JumppingState(MovementComponent2D context) : base(context) { }

    public override void OnStart()
    {
        base.OnStart();

        _currentblockTime = _grabBlockTime;
        _currentHoldTime = _grabHoldTime;
        Jump();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_rb.velocity.y < 0)
        {
            _context.ChangeMoveState(_context._fallingState);
        }

        if (_currentblockTime > 0) _currentblockTime -= Time.deltaTime;
        else if (_context.IsWallGrabable())
        {
            if (_currentHoldTime > 0) _currentHoldTime -= Time.deltaTime;
            else
            {
                if (_context.IsStickingWall()) _context.ChangeMoveState(_context._wallStickingState);
                else _context.ChangeMoveState(_context._wallGrabState);
            }
        }
        else _currentHoldTime = _grabHoldTime;
    }

    public void Move(Vector2 input)
    {
        _context.ApplyMovement(maxSpeed: _stats.moveMaxSpeed_air,
                               timeToReach: _stats.acceleration_air_sec,
                               timeToStop: _stats.deceleration_air_sec);
    }

    public void Jump()
    {
        if (_context._currentJumpCount >= _context.JumpMaxCount) return;

        _currentJumpTime = _context.JumpHoldTime;
        _context.IncreaseJumpCount();

        if (_rb.velocity.y <= 0) _rb.velocity = new Vector2(_rb.velocity.x, _stats.jumpForce);
    }

    public void ApplyGravity()
    {
        if (_context._isJumpPressed && _currentJumpTime > 0)
        {
            _rb.gravityScale = _context._defaultGravityScale;
            _rb.velocity += Vector2.up * _stats.jumpForce * Time.fixedDeltaTime;
            _currentJumpTime -= Time.fixedDeltaTime;
        }
        else if (!_context._isJumpPressed && _rb.velocity.y > 0)
        {
            _rb.gravityScale = _context._defaultGravityScale * _stats.lowJumpMultiplier;
        }
        //else
        //{
        //    _rb.gravityScale = _context._defaultGravityScale;
        //}
    }
}