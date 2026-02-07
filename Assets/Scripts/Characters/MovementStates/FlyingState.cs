using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingState : MovementStateBase, IMoveable
{
    public FlyingState(MovementComponent2D context) : base(context) { }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_stats != null && _stats.moveType == MoveType.OnlyFlying) return;

        if (_context.CheckGround())
        {
            _context.ChangeMoveState(_context._groundedState);
        }
    }

    public void Move(Vector2 input)
    {
        _context.ApplyFlyingMovement(maxSpeed: _context._currentMaxSpeed_flying,
                                    timeToReach: _stats.acceleration_flying_sec,
                                    timeToStop: _stats.deceleration_flying_sec);
    }
}