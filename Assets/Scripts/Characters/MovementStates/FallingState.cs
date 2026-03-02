using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingState : MovementStateBase, IMoveable, IJumpable
{
    private float _grabBlockTime = 0.1f;
    private float _grabHoldTime = 0.0f;
    private float _currentblockTime;
    private float _currentHoldTime;

    private float _startPosY;

    public FallingState(MovementComponent2D context) : base(context) { }

    public override void OnStart()
    {
        base.OnStart();

        _startPosY = _context.gameObject.transform.position.y;
        _currentblockTime = _grabBlockTime;
        _currentHoldTime = _grabHoldTime;

        _rb.gravityScale = _context._defaultGravityScale * _stats.fallMultiplier;
    }
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_context.CheckGround())
        {
            HandleFallingHit();

            if (_context.HasJumpBuffer())
            {
                _context.ClearJumpBuffer();
                _context.ChangeMoveState(_context._jumpingState);
                return;
            }

            return;
        }

        HandleGrabInteraction();
    }

    public void Move(Vector2 input)
    {
        _context.ApplyMovement(maxSpeed: _context._currentMaxSpeed_air,
                               timeToReach: _stats.acceleration_air_sec,
                               timeToStop: _stats.deceleration_air_sec);
    }

    public void Jump()
    {
        if (_context.CanCoyoteJump() &&
            _context._currentJumpCount < _context.JumpMaxCount)
        {
            _context.ChangeMoveState(_context._jumpingState);
        }
    }

    private void HandleFallingHit()
    {
        if (_context._character is IHitable hitCharacter)
        {
            float impactDistance = Mathf.Abs(_startPosY - _context.gameObject.transform.position.y);

            if (impactDistance > _stats.fallingHitDistance)
            {
                float fallDamage = (impactDistance - _stats.fallingHitDistance / 2) * _stats.fallingHitMultiplier;

                HitInfo hitInfo = new HitInfo();

                hitInfo.damage = fallDamage;
                hitInfo.hitType = HitType.FallingHit;
                hitInfo.causer = _context._groundHit.collider.gameObject;

                hitCharacter.TakeDamage(hitInfo);
            }
        }

        _context.ChangeMoveState(_context._groundedState);
    }

    private void HandleGrabInteraction()
    {
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
}