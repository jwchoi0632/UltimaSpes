using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirborneState : MovementStateBase, IMoveable
{
    public AirborneState(MovementComponent2D context) : base(context) { }


    public override void OnStart()
    {
        base.OnStart();
        _rb.gravityScale = _context._defaultGravityScale;
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public void Move(Vector2 input)
    {
        _context.ApplyMovement(maxSpeed: _context._currentMaxSpeed_air,
                               timeToReach: _stats.acceleration_air_sec,
                               timeToStop: _stats.deceleration_air_sec);
    }
}
