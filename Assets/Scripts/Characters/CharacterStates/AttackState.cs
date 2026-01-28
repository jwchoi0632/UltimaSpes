using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : CharacterStateBase
{
    public AttackState(CharacterStateMachine stateMachine) : base(stateMachine) { }

    public override void OnStart()
    {
        base.OnStart();

        _movement.SetMoveInput(Vector2.zero);
        _movement.SetCanFlip(false);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnFiexedUpdate()
    {
        base.OnFiexedUpdate();
    }

    public override void OnExit()
    {
        base.OnExit();

        _movement.SetCanFlip(true);
    }
}
