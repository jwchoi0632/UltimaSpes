using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingState : MovementStateBase, IMoveable, IJumpable, IGravityEffect
{
    private float _grabBlockTime = 0.1f;
    private float _grabHoldTime = 0.0f;
    private float _currentblockTime;
    private float _currentHoldTime;

    public FallingState(MovementComponent2D context) : base(context) { }

    public override void OnStart()
    {
        base.OnStart();

        _currentblockTime = _grabBlockTime;
        _currentHoldTime = _grabHoldTime;
    }
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_context.CheckGround())
        {
            if (_context._character is HitableCharacter hitCharacter)
            {
                float impactVelocity = Mathf.Abs(_rb.velocity.y);

                if (impactVelocity > _stats.fallingHitImpact)
                {
                    float fallDamage = (impactVelocity - _stats.fallingHitImpact) * _stats.fallingHitMultiplier;

                    HitInfo hitInfo = new HitInfo();

                    hitInfo.damage = fallDamage;
                    hitInfo.hitType = HitType.FallingHit;
                    hitInfo.causer = _context._groundHit.collider.gameObject;

                    hitCharacter.TakeDamage(hitInfo);
                }
            }

            _context.ChangeMoveState(_context._groundedState);
            return;
        }

        if (_currentblockTime > 0) _currentblockTime -= Time.deltaTime;
        else if (_context.IsWallGrabable())
        {
            if (_currentHoldTime > 0) _currentHoldTime -= Time.deltaTime;
            else
            {
                if (_context.IsStickingWall()) _context.ChangeMoveState(_context._wallStickingState);
                else _context.ChangeMoveState(_context._wallGrabState);
            }
        }
        else _currentHoldTime = _grabHoldTime;
    }

    public void Move(Vector2 input)
    {
        _context.ApplyMovement(maxSpeed: _context._currentMaxSpeed_air,
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