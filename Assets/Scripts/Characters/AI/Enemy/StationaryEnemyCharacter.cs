using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryEnemyCharacter : HitableEnemyCharacter, INoticeable, IChaseable
{
    [SerializeField] private GameObject _noticeSprite;

    public ChaseState _chaseState { get; private set; }
    public NoticeState _noticeState { get; private set; }

    public GameObject NoticeSprite => _noticeSprite;

    protected override void PostEnabled(Vector2 spawnPos)
    {
        base.PostEnabled(spawnPos);

        _movement.ChangeMoveState(_movement._stationaryState);
    }

    protected override void InitState()
    {
        base.InitState();

        _chaseState = new ChaseState(this);
        _noticeState = new NoticeState(this);
    }
}
