using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : ControllerBase
{
    private PlayerCharacter _player;

    protected override void InitComponent()
    {
        base.InitComponent();

        _player = _character as PlayerCharacter;
    }

    public void BindInputAction()
    {
        var actions = InputReader.Instance.inputActions.PlayerActionMap;

        InputReader.Instance.BindAction(actions.Move,
            started: () => HandleVerticalInteractionInput(actions.Move.ReadValue<Vector2>()),
            performed: () => HandleMoveInput(actions.Move.ReadValue<Vector2>(), true),
            canceled: () => HandleMoveInput(Vector2.zero, false));

        InputReader.Instance.BindAction(actions.Jump,
            started: () => HandleJumpInput(true),
            canceled: () => HandleJumpInput(false));

        InputReader.Instance.BindAction(actions.Melee,
            started: () => HandleAttackInput(AttackType.Melee, true),
            canceled: () => HandleAttackInput(AttackType.Melee, false));

        InputReader.Instance.BindAction(actions.Ranged,
            started: () => HandleAttackInput(AttackType.Range, true),
            canceled: () => HandleAttackInput(AttackType.Range, false));

        InputReader.Instance.BindAction(actions.Skill,
            started: () => HandleAttackInput(AttackType.Skill, true),
            canceled: () => HandleAttackInput(AttackType.Skill, false));

        InputReader.Instance.BindAction(actions.Swap,
            started: () => HandleSwapInput(true),
            canceled: () => HandleSwapInput(false));

        InputReader.Instance.BindAction(actions.Interaction,
            started: () => HandleInteractionInput(true),
            canceled: () => HandleInteractionInput(false));

        InputReader.Instance.BindAction(actions.Walk,
            started: () => HandleWalkModeInput(true),
            canceled: () => HandleWalkModeInput(false));

        InputReader.Instance.BindAction(actions.Menu,
            started: () => HandleMenuInput(true),
            canceled: () => HandleMenuInput(false));
    }

    private void HandleVerticalInteractionInput(Vector2 inputValue)
    {
        _stateMachine.OnStartMoveInput(inputValue);
    }

    private void HandleMoveInput(Vector2 inputValue, bool isPressed)
    {
        if (isPressed) _stateMachine.OnMoveInput(inputValue);
        else _stateMachine.OnEndMoveInput();
    }

    private void HandleJumpInput(bool isPressed)
    {
        if (isPressed) _stateMachine.OnJumpInput();
        else _stateMachine.OnEndJumpInput();
    }

    private void HandleAttackInput(AttackType type, bool isPressed)
    {
        if (isPressed) _stateMachine.OnAttackInput(type);
        else _stateMachine.OnEndAttackInput();
    }

    private void HandleSwapInput(bool isPressed)
    {
        if (isPressed) _stateMachine.OnSwapInput();
        else _stateMachine.OnEndSwapInput();
    }

    private void HandleInteractionInput(bool isPressed)
    {
        if (isPressed) _stateMachine.OnInteractionInput();
        else _stateMachine.OnEndInteractionInput();
    }

    private void HandleWalkModeInput(bool isPressed)
    {
        // TODO : movementComponent의 최대 이동속도 감소, 복귀
    }

    private void HandleMenuInput(bool isPressed)
    {
        // TODO : UI 구현 되면, Menu, Inventory 출력
    }
}