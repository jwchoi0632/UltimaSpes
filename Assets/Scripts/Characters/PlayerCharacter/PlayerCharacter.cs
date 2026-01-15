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
            canceled : () => movement.SetMoveInput(actions.Move.ReadValue<Vector2>()));

        InputReader.Instance.BindAction(actions.Jump,
            started: () => CheckJumpable(),
            canceled: () => movement.SetJumpInput(false));
    }

    void Update()
    {
        if (currentState == CharacterState.Jumping &&
            movement.IsJumping)
        {
            movement.DoJump();
        }
    }

    private void CheckJumpable()
    {
        if (movement.IsGrounded ||
            (!movement.IsGrounded && currentJumpCount < maxJumpCount))
        {
            movement.SetJumpInput(true);
            ChangeState(CharacterState.Jumping);
        }
    }

    protected override void Die()
    {
        Debug.Log("player die");
    }
}
