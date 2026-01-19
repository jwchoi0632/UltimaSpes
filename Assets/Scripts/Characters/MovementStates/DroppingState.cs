using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppingState : MovementStateBase, IMoveable, IGravityEffect
{
    private float dropTime = 0.4f;
    private float currentTime;
    public DroppingState(MovementComponent2D context) : base(context) { }

    public override void OnStart()
    {
        base.OnStart();

        currentTime = dropTime;
        Physics2D.IgnoreCollision(_context._mainCollider, _context._groundHit.collider, true);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (currentTime > 0.0f) currentTime -= Time.deltaTime;
        else _context.ChangeMoveState(_context._fallingState);
    }

    public override void OnExit()
    {
        base.OnExit();

        Physics2D.IgnoreCollision(_context._mainCollider, _context._groundHit.collider, false);
    }

    public void Move(Vector2 input)
    {
        _context.ApplyMovement(maxSpeed: _stats.moveMaxSpeed_air,
                               timeToReach: _stats.dropdownMultiplier,
                               timeToStop: _stats.dropdownMultiplier);
    }

    //public void Jump()
    //{

    //}

    public void ApplyGravity()
    {
        _rb.gravityScale = _context._defaultGravityScale * _stats.dropdownMultiplier;
    }
}
