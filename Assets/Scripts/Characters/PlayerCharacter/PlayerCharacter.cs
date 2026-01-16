using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : CharacterBase
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
            performed: () => movement.SetMoveInput(actions.Move.ReadValue<Vector2>()),
            canceled : () => movement.SetMoveInput(Vector2.zero));

        InputReader.Instance.BindAction(actions.Jump,
            started: () => CheckJumpType(),
            canceled: () => movement.EndJump());
    }

    void Update()
    {

    }

    private void CheckJumpType()
    {
        if (movement.ActionDropDown()) currentState = CharacterState.Falling;
        else if (movement.DoJump()) currentState = CharacterState.Jumping;
    }

    protected override void Die()
    {
        Debug.Log("player die");
    }
}
