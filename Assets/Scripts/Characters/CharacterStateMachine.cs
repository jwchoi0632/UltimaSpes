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
    public CharacterStatsBase _stats { get; private set; }

    public CharacterStateBase _currentState { get; private set; }
    private IAttackable _attackable;
    private IHitable _hitable;

    private IPatrolable _patrolable;
    private IChaseable _chaseable;
    private IIdleable _idleable;
    private IWaitable _waitable;
    private IReturnable _returnable;
    private INoticeable _noticeable;

    public bool _activeIFrame { get; private set; }

    public Action<Collision2D> OnCollisionEntered;

    private Coroutine _iframe;

    void Start()
    {
        _character = GetComponent<CharacterBase>();
        _movement = GetComponent<MovementComponent2D>();
        _stats = _character.Stats;

        _attackable = _character as IAttackable;
        _hitable = _character as IHitable;

        _patrolable = _character as IPatrolable;
        _chaseable = _character as IChaseable;
        _idleable = _character as IIdleable;
        _waitable = _character as IWaitable;
        _returnable = _character as IReturnable;
        _noticeable = _character as INoticeable;

        //ChangeState(_character._normalState);
    }

    public void ChangeState(CharacterStateBase state)
    {
        _currentState?.OnExit();

        if (state == null) return;

        _currentState = state;
        _currentState.OnStart();
    }

    public void SetIFrame(float duration)
    {
        if (_iframe != null) StopCoroutine(_iframe);

        if (gameObject.activeInHierarchy) _iframe = StartCoroutine(ApplyIFrame(duration));
    }

    private IEnumerator ApplyIFrame(float duration)
    {
        _activeIFrame = true;

        yield return new WaitForSeconds(duration);

        _activeIFrame = false;
    }

    public void OnHitEnd()
    {
        if (_hitable != null)
        {
            _hitable.PostHit();
        }
    }

    public void OnAttackEnd()
    {
        if (_attackable != null)
        {
            _attackable.PostAttack();
        }
    }

    public void OnHit(HitInfo hitInfo)
    {
        if (!_currentState.IsDamageable) return;
        if (_hitable == null) return;

        _hitable._hitState.SetHitInfo(hitInfo);
        ChangeState(_hitable._hitState);
    }

    public void OnAttack(WeaponDataBase weaponData)
    {
        if (!_currentState.IsAttackable) return;
        if (_attackable == null) return;
        if (weaponData == null) return;

        _attackable._attackState.SetAttackData(weaponData);

        ChangeState(_attackable._attackState);
    }

    public void OnIdle()
    {
        if (_idleable == null)
        {
            ChangeState(_character._normalState);
            return;
        }

        ChangeState(_idleable._idleState);
    }

    public void OnPatrol()
    {
        if (!_currentState.IsMoveable) return;

        if (_patrolable == null)
        {
            OnIdle();
            return;
        }

        ChangeState(_patrolable._patrolState);
    }

    public void OnChase()
    {
        if (_chaseable == null) return;
        if (!_currentState.IsMoveable) return;

        ChangeState(_chaseable._chaseState);
    }

    public void OnWait(float time, CharacterStateBase nextState = null)
    {
        if (_waitable == null)
        {
            if (nextState != null) ChangeState(nextState);
            else OnIdle();
                return;
        }

        _waitable._waitState.SetWaitTime(time);
        _waitable._waitState.SetNextState(nextState);

        ChangeState(_waitable._waitState);
    }

    public void OnNotice(float time)
    {
        if (_noticeable == null)
        {
            OnChase();
            return;
        }

        _noticeable._noticeState.SetNoticeTime(time);

        ChangeState(_noticeable._noticeState);
    }

    public void OnReturnToStart()
    {
        if (_returnable == null)
        {
            OnPatrol();
            return;
        }

        ChangeState(_returnable._returnState);
    }

    public void OnAttackInput(AttackType type)
    {
        if (_character.TryGetComponent<WeaponComponent>(out var weaponComp))
        {
            OnAttack(weaponComp.GetWeaponData(type));
        }
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
        if (_currentState.IsMoveable)
        {
            _movement.SetMoveInput(input);
        }
        
        if (_currentState is AttackState attackState)
        {
            attackState.SetAimInput(input.y);
        }
    }

    public void OnEndMoveInput()
    {
        if (_currentState.IsMoveable)
        {
            _movement.SetMoveInput(Vector2.zero);
        }
        
        if (_currentState is AttackState attackState)
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

    private void OnDisable()
    {
        if (_iframe != null)
        {
            StopCoroutine(_iframe);
            _iframe = null;
        }
    }
}