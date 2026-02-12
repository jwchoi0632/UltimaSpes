using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : CharacterStateBase
{
    private EnemyCharacterBase _character;
    private EnemyStats _enemyStats;
    private Vector2 _patrolTarget;
    private Vector2 _startPos;
    private IPatrolable _patrolable;

    public PatrolState(CharacterBase character) : base(character) 
    { 
        _character = _owner as EnemyCharacterBase;
        _enemyStats = _stats as EnemyStats;
        _patrolable = _character as IPatrolable;
    }

    public void SetStartPos(Vector2 pos) => _startPos = pos;

    public override void OnStart()
    {
        base.OnStart();

        SetNewPatrolTarget();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        _character._navigation.MoveTowards(_patrolTarget);

        if (_character._navigation.IsPathBlocked())
        {
            _character._navigation.Stop();
            _stateMachine.OnWait(2.0f, _patrolable?._patrolState);
            return;
        }

        float dist = Mathf.Abs(_patrolTarget.x - _owner.transform.position.x);

        if (dist < 0.2f)
        {
            _character._navigation.Stop();
            _stateMachine.OnWait(2.0f, _patrolable?._patrolState);
        }
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    private void SetNewPatrolTarget()
    {
        Vector2 targetPos = _startPos;
        float dir = _movement._isFacingRight ? 1 : -1;

        if (_enemyStats.patrolType == PatrolType.Range)
        {
            dir *= -1;
            targetPos += new Vector2(dir * _enemyStats.patrolRange, 0);
        }
        else
        {
            float min = -_enemyStats.patrolRange;
            float max = _enemyStats.patrolRange;

            if (_character._navigation.IsPathBlocked())
            {
                if (dir < 0) min = 0.1f;
                else max = -0.1f;
            }
                
            targetPos.x += Random.Range(min, max);
        }

        targetPos.y = (_enemyStats.moveType == MoveType.OnlyGround) ? 0 : Random.Range(-1f, 1f);

        _patrolTarget = targetPos;
    }
}