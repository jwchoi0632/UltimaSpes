using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IIdleable
{
    public IdleState _idleState { get; }
}

public interface IWaitable
{
    public WaitState _waitState { get; }
}

public interface IReturnable
{
    public ReturnToStartState _returnState { get; }
}

public interface INoticeable
{
    public NoticeState _noticeState { get; }
}

public interface IPatrolable
{
    public PatrolState _patrolState { get; }
}

public interface IChaseable
{
    public ChaseState _chaseState { get; }
}

public abstract class AICharacterBase : CharacterBase, IIdleable, IWaitable
{
    [Header("AI Settings")]
    [SerializeField] protected AIControllerBase _controller;

    public IdleState _idleState { get; private set; }
    public WaitState _waitState { get; private set; }

    public AIControllerBase AIController => _controller;

    //protected override void OnAwake()
    //{
    //    base.OnAwake();

    //}

    //protected override void OnStart()
    //{
    //    base.OnStart();

    //}

    public override void SetActiveCharacter(Vector2 spawnPos)
    {
        base.SetActiveCharacter(spawnPos);

        if (this is INoticeable noticeable) noticeable._noticeState.InitNotice();
        if (this is IReturnable returnable) returnable._returnState.SetReturnPoint(spawnPos);
        if (this is IPatrolable patrolable) patrolable._patrolState.SetStartPos(spawnPos);

        _controller?.Initialize();
    }

    protected override void InitState()
    {
        base.InitState();

        _idleState = new IdleState(this);
        _waitState = new WaitState(this);
    }

    protected override void InitComponents()
    {
        base.InitComponents();

        _controller = GetComponent<AIControllerBase>();
    }

    //protected override void Die()
    //{
        
    //}
}
