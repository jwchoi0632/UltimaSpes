using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AttackState : CharacterStateBase
{
    public AttackState(CharacterBase character) : base(character) { }

    private WeaponDataBase _currentData;
    private WeaponComponent weaponComp;

    private float _aimInput;
    private float _currentAimAngle;
    private float _currentAim;
    private float _currentCharge;
    private bool _attackPressed;

    private IAimable _aimable;
    private IChargeable _chargeable;
    private IMoveableOnAttack _moveableOnAttack; 
    private AttackContext _attackContext;

    public void SetAttackData(WeaponDataBase data) => _currentData = data;
    public void SetAimInput(float input_y) => _aimInput = input_y;

    public override void OnStart()
    {
        base.OnStart();

        if (_currentData == null)
        {
            PostAttack();
            return;
        }

        CastDataInterface();
        InitMovementOnState();
        InitAttackContext();
        
        SetAttackPressed(true);

        if (_aimable == null && _chargeable == null) OnAttackTrigger();
    }

    private void CastDataInterface()
    {
        _aimable = _currentData as IAimable;
        _chargeable = _currentData as IChargeable;
        _moveableOnAttack = _currentData as IMoveableOnAttack;
    }

    private void InitMovementOnState()
    {
        _moveable = (_moveableOnAttack != null);

        if (_moveable)
        {
            _movement.SetCurrentMaxSpeedOnGround(_moveableOnAttack.MaxSpeedOnGround);
            _movement.SetCurrentMaxSpeedInAir(_moveableOnAttack.MaxSpeedInAir);
        }
        else _movement.SetMoveInput(Vector2.zero);

        _movement.SetCanFlip(false);
    }

    private void InitAttackContext()
    {
        _attackContext = new AttackContext();

        _currentCharge = 0;
        _currentAim = 0;
        _currentAimAngle = 0;

        _owner.TryGetComponent<WeaponComponent>(out weaponComp);

        _attackContext.damageContext.baseDamage = _currentData.weaponDamage;
        _attackContext.direction = GetAimDirection();
    }

    public void OnAttackTrigger()
    {
        if (_currentData == null) return;

        if (weaponComp != null)
        {
            _attackContext.spawnPos = weaponComp.GetFirepoint();
        }

        if (_chargeable != null)
        {
            if (_chargeable.MinChargeTime <= _currentCharge)
            {
                Debug.Log("Charge Success");
                _attackContext.chargeRatio = _currentCharge / _chargeable.MaxChargeTime;
                _currentData.chargeAttackData?.performer.Excute(_owner, _currentData.chargeAttackData, _attackContext);
            }
            else
            {
                Debug.Log("Charge Fail");
            }
        }
        else
        {
            _currentData.normalAttackData?.performer.Excute(_owner, _currentData.normalAttackData, _attackContext);
        }

        PostAttack();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_chargeable != null &&
            _chargeable.MaxChargeTime > _currentCharge)
        {
            _currentCharge += Time.deltaTime;
        }

        if (_aimable != null)
        {
            if (_aimable.MinAimTime > _currentAim)
            {
                _currentAim += Time.deltaTime;
            }
            else
            {
                UpdateAim();
            }
        }
    }

    public void SetAttackPressed(bool pressed)
    {
        _attackPressed = pressed;

        if (!_attackPressed)
        {
            if (_chargeable != null || _aimable != null) OnAttackTrigger();
        }
    }

    public override void OnFiexedUpdate()
    {
        base.OnFiexedUpdate();
    }

    public override void OnExit()
    {
        base.OnExit();

        weaponComp.SetAimLinerEnable(false);

        _movement.SetCurrentMaxSpeedOnGround(_movement._maxSpeed_ground);
        _movement.SetCurrentMaxSpeedInAir(_movement._maxSpeed_air);

        _movement.SetCanFlip(true);
        _attackPressed = false;
        _currentData = null;
    }

    private void UpdateAim()
    {
        if (!weaponComp.IsEnabledAimLiner())
        {
            //_currentAimAngle = -90;
            weaponComp.SetAimLinerEnable(true);
        }

        if (_aimInput != 0)
        {
            float targetAngle = _aimInput * 90f;
            _currentAimAngle = Mathf.Lerp(_currentAimAngle, targetAngle, Time.deltaTime * _aimable.AimMultiplier);
        }

        _attackContext.direction = GetAimDirection();
        weaponComp?.UpdateAimLiner(_attackContext.direction);
    }

    public Vector2 GetAimDirection()
    {
        float lookDir = _owner._movement._isFacingRight ? 1f : -1f;

        float angleRad = _currentAimAngle * Mathf.Deg2Rad;

        return new Vector2(Mathf.Cos(angleRad) * lookDir, Mathf.Sin(angleRad));
    }

    private void PostAttack()
    {

        // TODO : 공격 후 후딜 처리
        if (weaponComp != null) weaponComp.SetCooldown(_currentData);
        _stateMachine.OnAttackEnd();
    }
}