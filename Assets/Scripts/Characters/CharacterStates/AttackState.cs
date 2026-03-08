using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AttackState : CharacterStateBase, IAimming
{
    public AttackState(CharacterBase character) : base(character) { }

    private WeaponDataBase _currentData;
    private WeaponComponent weaponComp;

    private float _aimInput;
    private float _currentAimAngle;
    private float _currentAim;
    private float _currentCharge;
    private float _recoveryTime;
    private bool _attackPressed;

    private IAimable _aimable;
    private IChargeable _chargeable;
    private IMoveableOnAttack _moveableOnAttack;
    private IInstallable _installable;
    private AttackContext _attackContext;

    public void SetAttackData(WeaponDataBase data) => _currentData = data;
    public void SetAimInput(float input_y) => _aimInput = input_y;

    public override void OnStart()
    {
        _stateName = "АјАн";
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

        _recoveryTime = 0;
    }

    private void CastDataInterface()
    {
        _aimable = _currentData as IAimable;
        _chargeable = _currentData as IChargeable;
        _moveableOnAttack = _currentData as IMoveableOnAttack;
        _installable = _currentData as IInstallable;
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

        if (weaponComp != null)
        {
            _attackContext.spawnPos = weaponComp.GetFirepoint();
        }

        _attackContext.damageContext.baseDamage = _currentData.weaponDamage;
        _attackContext.direction = GetAimDirection();
    }

    public void OnAttackTrigger()
    {
        if (_currentData == null) return;

        if (_chargeable != null)
        {
            if (_currentData.chargeAttackData == null) return;

            if (_chargeable.MinChargeTime <= _currentCharge)
            {
                Debug.Log("Charge Success");
                _attackContext.chargeRatio = _currentCharge / _chargeable.MaxChargeTime;
                _currentData.chargeAttackData.performer.Excute(_owner, _currentData.chargeAttackData, _attackContext);
                _recoveryTime = _currentData.chargeAttackData.recoveryTime;

                return;
            }
            else
            {
                Debug.Log("Charge Fail");
                _currentData.chargeAttackData.performer.Undo();
            }
        }

        if (_currentData.normalAttackData == null) return;

        _currentData.normalAttackData.performer.Excute(_owner, _currentData.normalAttackData, _attackContext);
        _recoveryTime = _currentData.normalAttackData.recoveryTime;

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

        if (_installable != null)
        {
            UpdateInstallGuide();
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
        weaponComp.SetInstallIndicatorEnable(false);

        _movement.SetCurrentMaxSpeedOnGround(_movement._maxSpeed_ground);
        _movement.SetCurrentMaxSpeedInAir(_movement._maxSpeed_air);

        _movement.SetCanFlip(true);
        _attackPressed = false;
        _currentData = null;
    }

    private void UpdateAim()
    {
        if (_aimInput != 0)
        {
            float targetAngle = _aimInput * 90f;
            _currentAimAngle = Mathf.Lerp(_currentAimAngle, targetAngle, Time.deltaTime * _aimable.AimMultiplier);
        }

        _attackContext.direction = GetAimDirection();
        weaponComp?.UpdateAimLiner(_attackContext.direction);
    }

    private void UpdateInstallGuide()
    {
        float lookDir = _owner._movement._isFacingRight ? 1f : -1f;
        Vector2 checkPos = (Vector2)_owner.transform.position + new Vector2(lookDir * _installable.Distance, _installable.VerticalWeight);

        _attackContext.isFailed = Physics2D.OverlapBox(checkPos, _installable.InstallSize, 0, _installable.ObstacleLayer);

        weaponComp.ShowInstallIndicator(checkPos, !_attackContext.isFailed);

        _attackContext.spawnPos = checkPos;
    }

    public Vector2 GetAimDirection()
    {
        float lookDir = _owner._movement._isFacingRight ? 1f : -1f;

        float angleRad = _currentAimAngle * Mathf.Deg2Rad;

        return new Vector2(Mathf.Cos(angleRad) * lookDir, Mathf.Sin(angleRad));
    }

    private void PostAttack()
    {
        if (weaponComp != null) weaponComp.SetCooldown(_currentData);
        _stateMachine.OnAttackEnd(_recoveryTime);
    }
}