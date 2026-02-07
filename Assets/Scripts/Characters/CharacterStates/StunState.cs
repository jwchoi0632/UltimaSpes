using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunState : CharacterStateBase
{
    private float _stunTime;

    public StunState(CharacterBase character) : base(character) { _moveable = false; }

    public void SetStunTime(float time) => _stunTime = time;

    public override void OnStart()
    {
        base.OnStart();

        _movement.SetMoveInput(Vector2.zero);
        _movement.SetCanFlip(false);
        Debug.Log("On Start Stun State");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_stunTime > GetStateDuration()) return;

        _stateMachine.OnHitEnd();
    }

    public override void OnExit()
    {
        base.OnExit();

        _movement.SetCanFlip(true);
        Debug.Log("On End Stun State");
    }
}
