using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToStartState : CharacterStateBase
{
    private EnemyCharacterBase _character;
    private Vector2 _returnPos;
    private IPatrolable _patrolable;
    private bool _hasResetPoint;

    public ReturnToStartState(CharacterBase character) : base(character) 
    { 
        _character = _owner as EnemyCharacterBase;
        _patrolable = _character as IPatrolable;
    }

    public void SetReturnPoint(Vector2 pos) => _returnPos = pos;

    public override void OnStart()
    {
        _stateName = "º¹±Í";
        base.OnStart();

        _hasResetPoint = false;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        _character._navigation.MoveTowards(_returnPos);

        if ((_character._navigation.IsPathBlocked() || GetStateDuration() > 5.0f) && !_hasResetPoint)
        {
            _hasResetPoint = true;

            Vector2 newPoint = _character._navigation.GetNearestValidPoint();
            //SetReturnPoint(newPoint);

            if (newPoint.y < _owner.transform.position.y) _movement.IsIgnoreHoverHeight(true);

            _character._navigation.Stop();
            return;
        }

        float dist = Vector2.Distance(_returnPos, _owner.transform.position);

        if (dist < 0.2f)
        {
            _character._navigation.Stop();
            _stateMachine.OnWait(2.0f, _patrolable?._patrolState);
        }
    }
}