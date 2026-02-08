using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class HitState : CharacterStateBase
{
    private HitInfo _hitInfo;
    private HitPolicy _currentPolicy;
    private IHitable _hitable;
    private IStunable _stunable;

    public HitPolicy CurrentPolicy => _currentPolicy;

    public HitState(CharacterBase character) : base(character)
    { 
        _stunable = _owner as IStunable;
        _hitable = _owner as IHitable;
    }

    public void SetHitInfo(HitInfo hitInfo) => _hitInfo = hitInfo;

    public override void OnStart()
    {
        base.OnStart();

        _movement.SetMoveInput(Vector2.zero);

        if (_hitable != null)
        {
            _currentPolicy = _hitable.HitData.GetPolicy(_hitInfo.hitType);

            if (_currentPolicy != null)
            {
                _damageable = _currentPolicy.canHit;
                _moveable = _currentPolicy.canMove;
                _movement.SetCanFlip(_currentPolicy.canFlip);
                _movement._rb.gravityScale = _currentPolicy.gravityScale;
                _stateMachine.SetIFrame(_currentPolicy.iFrame);
                _currentPolicy.action?.OnStart(_stateMachine);
            }
        }

        OnKnockback();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        float elapsed = GetStateDuration();

        if (elapsed < _hitInfo.hitDuration) return;

        if (_hitInfo.isStun && _stunable != null)
        {
            _stunable._stunState.SetStunTime(_hitInfo.stunDuration);
            _stateMachine.ChangeState(_stunable._stunState);
        }

        if (_currentPolicy != null)
        {
            _currentPolicy.action?.OnUpdate(_stateMachine, _hitInfo);
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        _movement.SetCanFlip(true);
        _movement._rb.gravityScale = _movement._defaultGravityScale;

        if (_currentPolicy != null)
        {
            _currentPolicy.action?.OnExit(_stateMachine);
        }
    }

    private void OnKnockback()
    {
        float diffX = _hitInfo.causer.transform.position.x - _owner.transform.position.x;

        if (Mathf.Abs(diffX) < 0.01f)
        {
            diffX = _movement._isFacingRight ? 1f : -1f;
        }

        float pushDir = diffX > 0 ? -1f : 1f;
        Vector2 force = new Vector2(_hitInfo.knockForce * pushDir, _hitInfo.launchForce);

        _movement.ApplyImpulse(force);
    }
}
