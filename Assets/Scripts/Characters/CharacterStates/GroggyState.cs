using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroggyState : CharacterStateBase
{
    private float _groggyDuration = 3.0f;

    public GroggyState(CharacterStateMachine stateMachine) : base(stateMachine) { }

    public override void OnStart()
    {
        base.OnStart();

        _movement.SetMoveInput(Vector2.zero);
        _movement.SetCanFlip(false);
        Debug.Log("On Start Groggy State");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_groggyDuration > GetStateDuration()) return;

        _stateMachine.ChangeState(_stateMachine._normalState);
    }

    public override void OnExit()
    {
        base.OnExit();

        _movement.SetCanFlip(true);
        Debug.Log("On End Groggy State");
    }
}
