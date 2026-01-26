using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : HitableCharacter
{
    protected override void OnAwake()
    {
        base.OnAwake();

    }

    protected override void OnStart()
    {
        base.OnStart();

        var actions = InputReader.Instance.inputActions.PlayerActionMap;

        InputReader.Instance.BindAction(actions.Move,
            performed: () => _stateMachine.OnMoveInput(actions.Move.ReadValue<Vector2>()),
            canceled : () => _stateMachine.OnEndMoveInput());

        InputReader.Instance.BindAction(actions.Jump,
            started: () => _stateMachine.OnJumpInput(),
            canceled: () => _stateMachine.OnEndJumpInput());
    }

    void Update()
    {

    }

    protected override void Die()
    {
        Debug.Log("player die");
    }
}
