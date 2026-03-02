using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroggyState : CharacterStateBase
{
    private float _groggyDuration = 3.0f;

    public GroggyState(CharacterBase character) : base(character) { _moveable = false; }

    public override void OnStart()
    {
        _stateName = "±×·Î±â";
        base.OnStart();

        _movement.SetMoveInput(Vector2.zero);
        _movement.SetCanFlip(false);
        Debug.Log("On Start Groggy State");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_groggyDuration > GetStateDuration()) return;

        _stateMachine.OnHitEnd();
    }

    public override void OnExit()
    {
        base.OnExit();

        _movement.SetCanFlip(true);
        Debug.Log("On End Groggy State");
    }
}
