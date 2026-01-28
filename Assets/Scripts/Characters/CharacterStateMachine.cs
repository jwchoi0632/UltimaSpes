using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling;
using UnityEngine;
using UnityEngine.Windows;

public class CharacterStateMachine : MonoBehaviour
{
    public CharacterBase _character { get; private set; }
    public MovementComponent2D _movement { get; private set; }
    public CharacterStats _stats { get; private set; }

    public CharacterStateBase _currentState { get; private set; }

    public NormalState _normalState { get; private set; }
    public AttackState _attackState { get; private set; }
    public HitState _hitState { get; private set; }
    public StunState _stunState { get; private set; }
    public GroggyState _groggyState { get; private set; }
    public DieState _dieState { get; private set; }

    public bool _activeIFrame { get; private set; }

    public Action<Collision2D> OnCollisionEntered;

    private IAttackable _attackable;

    void Start()
    {
        _character = GetComponent<CharacterBase>();
        _movement = GetComponent<MovementComponent2D>();
        _stats = _character.Stats;

        InitStateClass();

        ChangeState(_normalState);
    }

    public void ChangeState(CharacterStateBase state)
    {
        _currentState?.OnExit();

        if (state == null) return;

        _currentState = state;
        _currentState.OnStart();

        _attackable = _currentState as IAttackable;
    }

    public void SetIFrame(float duration) => StartCoroutine(ApplyIFrame(duration));

    private IEnumerator ApplyIFrame(float duration)
    {
        _activeIFrame = true;

        yield return new WaitForSeconds(duration);

        _activeIFrame = false;
    }

    public void OnAttack()
    {
        if (_attackable == null) return;

        _attackable.Attack();
    }

    public void OnHit(HitInfo hitInfo)
    {
        if (!_currentState.IsDamageable) return;

        _hitState.SetHitInfo(hitInfo);
        ChangeState(_hitState);
    }

    public void OnMoveInput(Vector2 input)
    {
        if (_currentState is IMoveableState) _movement.SetMoveInput(input);
        else if (_currentState is HitState hit)
        { 
            if (hit.CurrentPolicy.canMove) _movement.SetMoveInput(input);
        }
    }

    public void OnEndMoveInput()
    {
        if (_currentState is IMoveableState) _movement.SetMoveInput(Vector2.zero);
        else if (_currentState is HitState hit)
        {
            if (hit.CurrentPolicy.canMove) _movement.SetMoveInput(Vector2.zero);
        }
    }

    public void OnJumpInput()
    {
        if (_currentState is IJumpableState) _movement.StartJumppressed();
        else if (_currentState is HitState hit)
        {
            if (hit.CurrentPolicy.canMove) _movement.StartJumppressed();
        }
    }

    public void OnEndJumpInput()
    {
        if (_currentState is IJumpableState) _movement.EndJumppressed();
        else if (_currentState is HitState hit)
        {
            if (hit.CurrentPolicy.canMove) _movement.EndJumppressed();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionEntered?.Invoke(collision);
    }

    private void FixedUpdate()
    {
        _currentState?.OnFiexedUpdate();
    }

    void Update()
    {
        _currentState?.OnUpdate();
    }

    private void InitStateClass()
    {
        _normalState = new NormalState(this);
        _attackState = new AttackState(this);
        _hitState = new HitState(this);
        _stunState = new StunState(this);
        _groggyState = new GroggyState(this);
        _dieState = new DieState(this);
    }
}