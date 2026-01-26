using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunState : CharacterStateBase, IDamageable
{
    private float _stunTime;

    public StunState(CharacterStateMachine stateMachine) : base(stateMachine) { }

    public void SetStunTime(float time) => _stunTime = time;

    public override void OnStart()
    {
        base.OnStart();

        _movement.SetCanFlip(false);
        Debug.Log("On Start Stun State");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_stunTime > GetStateDuration()) return;

        _stateMachine.ChangeState(_stateMachine._normalState);
    }

    public override void OnExit()
    {
        base.OnExit();

        _movement.SetCanFlip(true);
        Debug.Log("On End Stun State");
    }
}
