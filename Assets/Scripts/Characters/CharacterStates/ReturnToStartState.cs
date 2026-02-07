using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToStartState : CharacterStateBase
{
    private EnemyCharacterBase _character;
    private Vector2 _returnPos;
    private IPatrolable _patrolable;

    public ReturnToStartState(CharacterBase character) : base(character) 
    { 
        _character = _owner as EnemyCharacterBase;
        _patrolable = _character as IPatrolable;
    }

    public void SetReturnPoint(Vector2 pos) => _returnPos = pos;

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_character._navigation.IsPathBlocked())
        {
            _character._navigation.Stop();
            _stateMachine.OnWait(2.0f, _patrolable?._patrolState);
            return;
        }

        _character._navigation.MoveTowards(_returnPos);

        float dist = Vector2.Distance(_returnPos, _owner.transform.position);

        if (dist < 0.2f)
        {
            _character._navigation.Stop();
            _stateMachine.OnWait(2.0f, _patrolable?._patrolState);
        }
    }
}