using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class HitState : CharacterStateBase
{
    private HitInfo _hitInfo;
    private bool _isLaunch;

    public HitState(CharacterStateMachine stateMachine) : base(stateMachine) { }

    public void SetHitInfo(HitInfo hitInfo) => _hitInfo = hitInfo;

    public override void OnStart()
    {
        base.OnStart();

        _movement.SetCanFlip(false);

        float diffX = _hitInfo.causer.transform.position.x - _owner.transform.position.x;

        if (Mathf.Abs(diffX) < 0.01f)
        {
            diffX = _movement._isFacingRight ? 1f : -1f;
        }

        float pushDir = diffX > 0 ? -1f : 1f;
        Vector2 force = new Vector2(_hitInfo.knockForce * pushDir, _hitInfo.launchForce);

        _isLaunch = _hitInfo.launchForce > 0.1f;

        _movement.ApplyImpulse(force);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        float elapsed = GetStateDuration();

        if (elapsed < _hitInfo.hitDuration) return;

        SelectNextState();

        //if (_isLaunch)
        //{
        //    if (_movement.CheckGround() && _movement._rb.velocity.y <= 0.1f)
        //    {
        //        SelectNextState();
        //    }
        //}
        //else
        //{
        //    SelectNextState();
        //}
    }

    public override void OnExit()
    {
        base.OnExit();

        _movement.SetCanFlip(true);
    }

    private void SelectNextState()
    {
        if (_isLaunch)
        {
            
        }
        else if (_hitInfo.isStun)
        {
            _stateMachine._stunState.SetStunTime(_hitInfo.stunDuration);
            _stateMachine.ChangeState(_stateMachine._stunState);
        }
        else
        {
            _stateMachine.ChangeState(_stateMachine._normalState);
        }
    }
}
