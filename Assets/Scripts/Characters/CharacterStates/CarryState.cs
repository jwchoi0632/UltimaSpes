using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryState : CharacterStateBase
{
    private IInteractable _target;
    private Transform _holdSocket;

    public CarryState(CharacterBase character) : base(character) { }

    public void SetCarryableObject(IInteractable target) => _target = target;
    public void SetHoldSocket(Transform socket) => _holdSocket = socket;

    public void OnThrow()
    {
        if (_target != null)
        {
            _target.gameObject.transform.SetParent(null, false);
            _target.OnInteraction(_owner.gameObject, InteractionType.Throw);
            _target = null;
        }
        
        _stateMachine.ChangeState(_owner._normalState);
    }

    public override void OnStart()
    {
        base.OnStart();

        if (_target == null) _stateMachine.ChangeState(_owner._normalState);

        _target.gameObject.transform.SetParent(_holdSocket, false);
    }

    public override void OnExit()
    {
        base.OnExit();

        if (_target == null) return;

        _target.gameObject.transform.SetParent(null, false);
        _target = null;
    }
}