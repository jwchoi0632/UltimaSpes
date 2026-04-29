using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyCharacter : HitableEnemyCharacter, IChaseable, INoticeable, IReturnable
{
    [SerializeField] private GameObject _noticeSprite;

    public ChaseState _chaseState { get; private set; }
    public NoticeState _noticeState { get; private set; }
    public ReturnToStartState _returnState { get; private set; }

    public GameObject NoticeSprite => _noticeSprite;

    protected override void PostEnabled(Vector2 spawnPos)
    {
        base.PostEnabled(spawnPos);

        _movement.StartFlying();
    }

    protected override void Die()
    {
        base.Die();

        _movement.EndFlying();
    }

    protected override void InitState()
    {
        base.InitState();

        _chaseState = new ChaseState(this);
        _noticeState = new NoticeState(this);
        _returnState = new ReturnToStartState(this);
    }
}
