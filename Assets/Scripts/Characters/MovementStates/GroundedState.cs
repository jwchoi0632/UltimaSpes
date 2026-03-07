using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedState : MovementStateBase, IMoveable, IJumpable
{
    public GroundedState(MovementComponent2D context) : base(context) { }

    public override void OnStart()
    {
        base.OnStart();

        _context.ResetJumpCount();
        _rb.gravityScale = _context._defaultGravityScale;
        //_rb.velocity = new Vector2(_rb.velocity.x, -0.1f);
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        if (!_context.CheckGround())
        {
            _context.UpdateCoyoteTime();
            _context.ChangeMoveState(_context._fallingState);
            return;
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        
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
}