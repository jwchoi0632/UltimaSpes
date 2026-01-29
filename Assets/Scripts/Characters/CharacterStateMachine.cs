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
    private IAttackable _attackable;
    private IHitable _hitable;

    public bool _activeIFrame { get; private set; }

    public Action<Collision2D> OnCollisionEntered;

    void Start()
    {
        _character = GetComponent<CharacterBase>();
        _movement = GetComponent<MovementComponent2D>();
        _stats = _character.Stats;

        _attackable = _character as IAttackable;
        _hitable = _character as IHitable;

        ChangeState(_character._normalState);
    }

    public void ChangeState(CharacterStateBase state)
    {
        _currentState?.OnExit();

        if (state == null) return;

        _currentState = state;
        _currentState.OnStart();
    }

    public void SetIFrame(float duration) => StartCoroutine(ApplyIFrame(duration));

    private IEnumerator ApplyIFrame(float duration)
    {
        _activeIFrame = true;

        yield return new WaitForSeconds(duration);

        _activeIFrame = false;
    }

    public void OnHit(HitInfo hitInfo)
    {
        if (!_currentState.IsDamageable) return;

        if (_hitable == null) return;

        _hitable._hitState.SetHitInfo(hitInfo);
        ChangeState(_hitable._hitState);
    }

    public void OnAttackInput(AttackType type)
    {
        if (!_currentState.IsAttackable) return;

        if (_attackable == null) return;

        if (_character.TryGetComponent<WeaponComponent>(out var weaponComp))
        {
            _attackable._attackState.SetAttackData(weaponComp.GetWeaponData(type));
        }

        ChangeState(_attackable._attackState);
    }

    public void OnEndAttackInput()
    {
        if (_currentState is AttackState attackState)
        {
            attackState.SetAttackPressed(false);
        }
    }

    public void OnMoveInput(Vector2 input)
    {
        if (_currentState.IsMoveable) _movement.SetMoveInput(input);
        else if (_currentState is AttackState attackState)
        {
            attackState.SetAimInput(input.y);
        }
    }

    public void OnEndMoveInput()
    {
        if (_currentState.IsMoveable) _movement.SetMoveInput(Vector2.zero);
        else if (_currentState is AttackState attackState)
        {
            attackState.SetAimInput(0);
        }
    }

    public void OnJumpInput()
    {
        if (_currentState.IsMoveable) _movement.StartJumppressed();
    }

    public void OnEndJumpInput()
    {
        if (_currentState.IsMoveable) _movement.EndJumppressed();
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
}