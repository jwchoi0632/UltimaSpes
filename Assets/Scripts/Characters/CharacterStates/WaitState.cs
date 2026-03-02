using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitState : CharacterStateBase
{
    private float _currentTime;
    private CharacterStateBase _nextState;

    public WaitState(CharacterBase character) : base(character) { }

    public void SetWaitTime(float time) => _currentTime = time;
    public void SetNextState(CharacterStateBase nextState) => _nextState = nextState;

    public override void OnStart()
    {
        _stateName = "상태 전이 기다림";
        base.OnStart();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_currentTime > 0) _currentTime -= Time.deltaTime;
        else
        {
            if (_nextState != null) _stateMachine.ChangeState(_nextState);
            else _stateMachine.OnIdle();
        }
    }
}