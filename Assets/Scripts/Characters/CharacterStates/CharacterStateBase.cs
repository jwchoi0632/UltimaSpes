using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable { void Attack(); }
public interface IMoveableState { }
public interface IJumpableState { }

public abstract class CharacterStateBase
{
    protected CharacterBase _owner;
    protected CharacterStateMachine _stateMachine;
    protected MovementComponent2D _movement;
    protected CharacterStats _stats;

    protected float _stateStartTime;
    protected bool _damageable = true;

    public bool IsDamageable => _damageable;

    public CharacterStateBase(CharacterStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
        _stats = _stateMachine._stats;
        _owner = _stateMachine._character;
        _movement = _stateMachine._movement;
    }

    public virtual void OnStart()
    {
        _stateStartTime = Time.time;

        _movement.EndJumppressed();
    }

    public virtual void OnUpdate() { }
    public virtual void OnFiexedUpdate() { }
    public virtual void OnExit() { }

    public float GetStateDuration() => Time.time - _stateStartTime;
}
