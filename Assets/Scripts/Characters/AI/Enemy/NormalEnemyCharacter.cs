using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEnemyCharacter : HitableEnemyCharacter, IPatrolable, IChaseable, INoticeable
{
    public PatrolState _patrolState { get; private set; }
    public ChaseState _chaseState { get; private set; }
    public NoticeState _noticeState { get; private set; }

    protected override void InitState()
    {
        base.InitState();

        _patrolState = new PatrolState(this);
        _chaseState = new ChaseState(this);
        _noticeState = new NoticeState(this);
    }
}