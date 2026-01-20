using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallStickingState : MovementStateBase, IMoveable, IJumpable
{
    public WallStickingState(MovementComponent2D context) : base(context) { }

    public override void OnStart()
    {
        base.OnStart();

        _rb.gravityScale = 0;
        _rb.velocity = Vector2.zero;
        _context.ResetJumpCount();

        SnapToWall();
    }

    //public override void OnUpdate()
    //{
    //    base.OnUpdate();

    //    if (_context.CheckGround())
    //    {
    //        _context.ChangeMoveState(_context._groundedState);
    //    }
    //}

    public override void OnExit()
    {
        base.OnExit();
    }

    public void Move(Vector2 input)
    {
        //_context.ApplyMovement(maxSpeed: _stats.moveMaxSpeed_grab,
        //                       isHorizontal: false,
        //                       timeToReach: _stats.acceleration_grab_sec,
        //                       timeToStop: _stats.deceleration_grab_sec);
    }

    public void Jump()
    {
        _rb.velocity = new Vector2(_context._moveInput.x * _stats.jumpForce_wallSticking_vertical, _stats.jumpForce_wallSticking_horizontal);
        _context.ChangeMoveState(_context._jumpingState);
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
