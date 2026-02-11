using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class CharacterStateBase
{
    protected CharacterBase _owner;
    protected CharacterStateMachine _stateMachine;
    protected MovementComponent2D _movement;
    protected CharacterStatsBase _stats;

    protected float _stateStartTime;
    protected bool _damageable = true;
    protected bool _moveable = true;
    protected bool _attackable = false;
    protected bool _interactable = false;

    public bool IsDamageable => _damageable;
    public bool IsMoveable => _moveable;
    public bool IsAttackable => _attackable;
    public bool IsInteractable => _interactable;

    public CharacterStateBase(CharacterBase character)
    {
        _owner = character;
        _stateMachine = _owner._stateMachine;
        _movement = _owner._movement;
        _stats = _owner.Stats;
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
