using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedState : MovementStateBase, IMoveable, IJumpable, IGravityEffect
{
    public GroundedState(MovementComponent2D context) : base(context) { }

    public override void OnStart()
    {
        base.OnStart();

        _context.ResetJumpCount();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (!_context.CheckGround())
        {
            _context.ChangeMoveState(_context._fallingState);
            return;
        }
    }

    public void Move(Vector2 input)
    {
        _context.ApplyMovement(maxSpeed: _context._currentMaxSpeed,
                               timeToReach: _stats.acceleration_sec,
                               timeToStop: _stats.deceleration_sec);
    }

    public void Jump()
    {
        if (_context.IsOnThinPlatform() && _context._moveInput.y < 0)
        {
            _context.ChangeMoveState(_context._droppingState);
        }
        else
        {
            _context.ChangeMoveState(_context._jumpingState);
        }
    }

    public void ApplyGravity()
    {
        _rb.gravityScale = _context._defaultGravityScale;
    }
}