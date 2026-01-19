using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGrabState : MovementStateBase, IMoveable, IGravityEffect, IJumpable
{
    public WallGrabState(MovementComponent2D context) : base(context) { }

    public override void OnStart()
    {
        base.OnStart();

        _rb.gravityScale = 0;
        _rb.velocity = Vector2.zero;
        _context.ResetJumpCount();

        SnapToWall();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_context.CheckGround())
        {
            _context.ChangeMoveState(_context._groundedState);
        }

        if (!_context.IsWallGrabable())
        {
            _context.ChangeMoveState(_context._fallingState);
        }
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public void Move(Vector2 input)
    {
        if(!_context.IsPushing())
        {
            _context.ChangeMoveState(_context._fallingState);
            return;
        }

        _context.ApplyMovement(maxSpeed: _stats.moveMaxSpeed_grab,
                               isHorizontal: false,
                               timeToReach: _stats.acceleration_grab_sec,
                               timeToStop: _stats.deceleration_grab_sec);
    }

    public void Jump()
    {
        float jumpDirection = _context._isFacingRight ? -1f : 1f;

        Vector2 jumpForce = new Vector2(jumpDirection * _stats.grapJumpHorizontal, _rb.velocity.y);

        _rb.velocity = jumpForce;

        _context.SetMoveInput(new Vector2(jumpDirection, _rb.velocity.y));
        _context.ChangeMoveState(_context._jumpingState);
    }

    public void ApplyGravity()
    {
        
    }

    private void SnapToWall()
    {
        if (_context._wallHit.collider != null)
        {
            float direction = _context._isFacingRight ? 1 : -1;
            float targetX = _context._wallHit.point.x - (direction * _context._mainCollider.bounds.extents.x);

            _rb.position = new Vector2(targetX, _rb.position.y);
        }
    }
}
