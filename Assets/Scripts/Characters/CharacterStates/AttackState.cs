using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : CharacterStateBase
{
    public AttackState(CharacterBase character) : base(character) { _moveable = false; }

    private WeaponDataBase _currentData;
    private WeaponComponent weaponComp;

    private float _aimInput;
    private float _currentAimAngle;
    private float _currentCharge;
    private bool _attackPressed;

    public void SetAttackData(WeaponDataBase data) => _currentData = data;
    public void SetAimInput(float input_y) => _aimInput = input_y;

    public override void OnStart()
    {
        base.OnStart();

        _movement.SetMoveInput(Vector2.zero);
        _movement.SetCanFlip(false);

        SetAttackPressed(true);

        _currentCharge = 0;
        _currentAimAngle = -90;

        if (_owner.TryGetComponent<WeaponComponent>(out weaponComp))
        {
            if (_currentData.isAimable)
            {
                weaponComp.UpdateAimLiner(GetAimDirection());
                weaponComp.SetAimLinerEnable(true);
            }
        }
    }

    public void OnAttackTrigger()
    {
        if (_currentData == null) return;

        if (_currentData.isChargeable)
        {
            if (_currentData.chargeAttackData != null &&
                _currentData.chargeAttackData.minChargeTime <= _currentCharge)
            {
                Debug.Log("Charge Success");
                _currentData.chargeAttackData.performer.Excute(_owner, _currentData.chargeAttackData);
            }
            else
            {
                Debug.Log("Charge Fail");
                _stateMachine.ChangeState(_owner._normalState);
            }
        }
        else if (_currentData.normalAttackData != null)
        {
            _currentData.normalAttackData.performer.Excute(_owner, _currentData.normalAttackData);
        }

        PostAttack();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_currentData == null) return;

        if (_currentData.isChargeable &&
            _currentCharge <= _currentData.chargeAttackData.maxChargeTime)
        {
            _currentCharge += Time.deltaTime;
        }

        if (_currentData.isAimable)
        {
            UpdateAim();
        }
    }

    public void SetAttackPressed(bool pressed)
    {
        _attackPressed = pressed;

        if (!_attackPressed)
        {
            OnAttackTrigger();
        }
    }

    public override void OnFiexedUpdate()
    {
        base.OnFiexedUpdate();
    }

    public override void OnExit()
    {
        base.OnExit();

        _movement.SetCanFlip(true);
        _currentData = null;
    }

    private void UpdateAim()
    {
        if (_aimInput == 0) return;

        float targetAngle = _aimInput * 90f;

        _currentAimAngle = Mathf.Lerp(_currentAimAngle, targetAngle, Time.deltaTime * _currentData.aimMultiPlier);

        Vector2 dir = GetAimDirection();

        weaponComp?.UpdateAimLiner(dir);
    }

    public Vector2 GetAimDirection()
    {
        float lookDir = _owner._movement._isFacingRight ? 1f : -1f;

        float angleRad = _currentAimAngle * Mathf.Deg2Rad;

        return new Vector2(Mathf.Cos(angleRad) * lookDir, Mathf.Sin(angleRad));
    }

    private void PostAttack()
    {
        weaponComp.SetAimLinerEnable(false);
        // TODO : 공격 후 후딜 처리
        _stateMachine.ChangeState(_owner._normalState);
    }
}