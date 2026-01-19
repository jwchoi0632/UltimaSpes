using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingState : MovementStateBase, IMoveable, IGravityEffect, IJumpable
{
    public ClimbingState(MovementComponent2D context) : base(context) { }

    public override void OnStart()
    {
        base.OnStart();

        _rb.gravityScale = 0;
        _rb.velocity = Vector2.zero;
        _context.ResetJumpCount();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_context.CheckGround())
        {
            _context.ChangeMoveState(_context._groundedState);
        }
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public void Move(Vector2 input)
    {
        _context.ApplyMovement(maxSpeed: _stats.moveMaxSpeed_grab,
                               isHorizontal: false,
                               timeToReach: _stats.acceleration_grab_sec,
                               timeToStop: _stats.deceleration_grab_sec);
    }

    public void Jump()
    {
        _context.ChangeMoveState(_context._jumpingState);
    }

    public void ApplyGravity()
    {
        
    }
}
