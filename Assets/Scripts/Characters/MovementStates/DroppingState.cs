using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppingState : MovementStateBase, IMoveable
{
    private float dropTime = 0.4f;
    private float currentTime;
    private Collider2D _ignoreCollider;
    public DroppingState(MovementComponent2D context) : base(context) { }

    public override void OnStart()
    {
        base.OnStart();

        currentTime = dropTime;
        _ignoreCollider = _context._groundHit.collider;

        _rb.gravityScale = _context._defaultGravityScale * _stats.dropdownMultiplier;

        if (_ignoreCollider != null)
        {
            Physics2D.IgnoreCollision(_context._mainCollider, _ignoreCollider, true);
        }
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

        if (_ignoreCollider != null)
        {
            Physics2D.IgnoreCollision(_context._mainCollider, _ignoreCollider, false);
            _ignoreCollider = null;
        }
    }

    public void Move(Vector2 input)
    {
        _context.ApplyMovement(maxSpeed: _context._currentMaxSpeed_air,
                               timeToReach: _stats.dropdownMultiplier,
                               timeToStop: _stats.dropdownMultiplier);
    }
}
