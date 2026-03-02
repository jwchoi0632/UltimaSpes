using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderState : CharacterStateBase
{
    private IInteractable _ladder;

    public LadderState(CharacterBase character) : base(character) 
    { 
        _attackable = false; _interactable = false; 
    }

    public void SetLadderObj(IInteractable ladder) => _ladder = ladder;

    public override void OnStart()
    {
        _stateName = "사다리 타기";
        base.OnStart();

        if (_stateMachine._interaction == null || _ladder == null)
        {
            _owner.ResetState();
            return;
        }

        Vector2 targetPos = _movement._rb.position;
        targetPos.x = _ladder.gameObject.transform.position.x;
        _movement._rb.position = targetPos;

        _movement.ChangeMoveState(_movement._climbingState);
    }

    public override void OnFiexedUpdate()
    {
        base.OnFiexedUpdate();

        bool descending = _movement._moveInput.y < 0;

        if (_stateMachine._interaction.IsOnLadder(descending) == null)
        {
            if (!descending)
            {
                HandleLedgeClimb();
            }

            _owner.ResetState();
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        _movement.OnEndClimb();
        _ladder = null;
    }

    private void HandleLedgeClimb()
    {

    }
}