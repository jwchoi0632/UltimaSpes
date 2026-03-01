using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Profiling;
using UnityEngine;
using UnityEngine.Windows;
using static UnityEngine.GraphicsBuffer;

public class CharacterStateMachine : MonoBehaviour
{
    public CharacterBase _character { get; private set; }
    public MovementComponent2D _movement { get; private set; }
    public CharacterStatsBase _stats { get; private set; }
    public InteractionComponent _interaction { get; private set; }

    public CharacterStateBase _currentState { get; private set; }
    private IAttackable _attackable;
    private IHitable _hitable;

    private IPatrolable _patrolable;
    private IChaseable _chaseable;
    private IIdleable _idleable;
    private IWaitable _waitable;
    private IReturnable _returnable;
    private INoticeable _noticeable;

    private ICarryable _carryable;
    private IPushable _pushable;

    public bool _activeIFrame { get; private set; }

    public Action<Collision2D> OnCollisionEntered;

    private Coroutine _iframe;

    private bool _isInteractionPressed = false;
    private bool _isSwapPressed = false;
    private bool _isMovePressed = false;

    private IAimming _aimming;

    void Start()
    {
        _character = GetComponent<CharacterBase>();
        _movement = GetComponent<MovementComponent2D>();
        _interaction = GetComponent<InteractionComponent>();

        _stats = _character.Stats;

        _attackable = _character as IAttackable;
        _hitable = _character as IHitable;

        _patrolable = _character as IPatrolable;
        _chaseable = _character as IChaseable;
        _idleable = _character as IIdleable;
        _waitable = _character as IWaitable;
        _returnable = _character as IReturnable;
        _noticeable = _character as INoticeable;

        _carryable = _character as ICarryable;
        _pushable = _character as IPushable;
    }

    public void ChangeState(CharacterStateBase state)
    {
        _currentState?.OnExit();

        if (state == null) return;

        _currentState = state;
        _currentState.OnStart();

        _interaction?.SetInteractionEnable(_currentState.IsInteractable);

        _aimming = _currentState as IAimming;
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
            if (_isSwapPressed) weaponComp.Swap(type);
            else OnAttack(weaponComp.GetWeaponData(type));
        }
    }

    public void OnEndAttackInput()
    {
        if (_currentState is AttackState attackState)
        {
            attackState.SetAttackPressed(false);
        }
    }

    public void OnStartMoveInput(Vector2 input)
    {
        if (_currentState.IsInteractable
            && CheckInteraction(input)) return;

        OnMoveInput(input);
    }

    public void OnMoveInput(Vector2 input)
    {
        _isMovePressed = true;

        if (_currentState.IsMoveable)
        {
            _movement.SetMoveInput(input);
        }
        
        if (_aimming != null)
        {
            _aimming.SetAimInput(input.y);
        }
    }

    public void OnEndMoveInput()
    {
        _isMovePressed = false;

        if (_currentState is PushState)
        {
            _pushable._pushState.OnCancled();
        }

        if (_currentState.IsMoveable)
        {
            _movement.SetMoveInput(Vector2.zero);
        }

        if (_aimming != null)
        {
            _aimming.SetAimInput(0);
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

    public void OnInteractionInput()
    {
        if (_currentState is CarryState carry) // test
        {
            OnThrowInput();
            return;
        }

        if (!_currentState.IsInteractable) return;

        _isInteractionPressed = true;

        if (_interaction != null) _interaction.OnInteraction();
    }

    public void OnEndInteractionInput()
    {
        _isInteractionPressed = false;
    }

    public void OnSwapInput()
    {
        if (!_currentState.IsAttackable) return;

        _isSwapPressed = true;
    }

    public void OnEndSwapInput()
    {
        _isSwapPressed = false;
    }

    public void OnThrowInput()
    {
        if (_currentState is CarryState carry)
        {
            _interaction?.ClearCarryableObject();
            carry.OnThrow();
        }
    }

    private void ApplyPushInteraction(float inputx)
    {
        if (_pushable == null) return;
        if (inputx < 0.1f && inputx > -0.1f)
        {
            _pushable._pushState.OnCancled();
            return;
        }

        float dir = _movement._isFacingRight ? 1 : -1;

        if (dir * inputx < 0)
        {
            _pushable._pushState.OnCancled();
            return;
        }

        IInteractable target = _interaction.GetPushTarget(inputx);
        float pushRange = _interaction.GetPushSensorRange();

        _pushable._pushState.SetPushTarget(target, pushRange);

        if (target != null)
        {
            if (!(_currentState is PushState))
            {
                ChangeState(_pushable._pushState);
            }
        }
    }

    private bool CheckInteraction(Vector2 input)
    {
        if (_interaction == null) return false;

        if (input.y < -0.5f)
        {
            if (_carryable != null && _isInteractionPressed && _movement.CheckGround())
            {
                bool result = _interaction.OnCarryInteraction();

                if (result)
                {
                    _carryable._carryState.SetHoldSocket(_carryable.CarryHoldSocket);
                    _carryable._carryState.SetCarryableObject(_interaction.GetCarryableObject());
                    ChangeState(_carryable._carryState);
                }
                
                return result;
            }

            return _interaction.OnDownDirectionInteraction();
        }
        else if (input.y > 0.5f)
        {
            return _interaction.OnUpDirectionInteraction();
        }

        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionEntered?.Invoke(collision);
    }

    private void FixedUpdate()
    {
        _currentState?.OnFiexedUpdate();

        if (_isMovePressed) ApplyPushInteraction(_movement._moveInput.x);
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