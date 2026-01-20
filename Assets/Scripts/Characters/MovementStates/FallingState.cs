using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingState : MovementStateBase, IMoveable, IJumpable, IGravityEffect
{
    private float _grabBlockTime = 0.3f;
    private float _currentblockTime;
    public FallingState(MovementComponent2D context) : base(context) { }

    public override void OnStart()
    {
        base.OnStart();

        _currentblockTime = _grabBlockTime;
    }
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_context.CheckGround())
        {
            _context.ChangeMoveState(_context._groundedState);
            return;
        }

        if (_currentblockTime > 0) _currentblockTime -= Time.deltaTime;
        else if (_context.IsWallGrabable())
        {
            if (_context.IsStickingWall()) _context.ChangeMoveState(_context._wallStickingState);
            else _context.ChangeMoveState(_context._wallGrabState);
        }
    }

    public void Move(Vector2 input)
    {
        _context.ApplyMovement(maxSpeed: _stats.moveMaxSpeed_air,
                               timeToReach: _stats.acceleration_air_sec,
                               timeToStop: _stats.deceleration_air_sec);
    }

    public void Jump()
    {
        if (_context._currentJumpCount < _context.JumpMaxCount)
        {
            _context.ChangeMoveState(_context._jumpingState);
        }
    }

    public void ApplyGravity()
    {
        _rb.gravityScale = _context._defaultGravityScale * _stats.fallMultiplier;
    }
}