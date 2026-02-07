using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeaponComponent))]
public class PlayerCharacter : CharacterBase, IAttackable, IStunable, IGroggyable, IHitable
{
    [SerializeField] protected HitDatabase _hitDatabase;

    public AttackState _attackState { get; private set; }
    public StunState _stunState { get; private set; }
    public GroggyState _groggyState { get; private set; }
    public HitState _hitState { get; private set; }

    public WeaponComponent _weaponComponent { get; private set; }

    public HitDatabase HitData => _hitDatabase;

    protected override void OnAwake()
    {
        base.OnAwake();
        
        _weaponComponent = GetComponent<WeaponComponent>();
    }

    protected override void OnStart()
    {
        base.OnStart();

        BindInputAction();
        _stateMachine.ChangeState(_normalState);
    }

    void Update()
    {

    }

    protected override void Die()
    {
        Debug.Log("player die");
    }

    protected override void InitState()
    {
        base.InitState();

        _attackState = new AttackState(this);
        _stunState = new StunState(this);
        _groggyState = new GroggyState(this);
        _hitState = new HitState(this);
    }

    public void ApplyDamage(IHitable target, DamageContext damageContext, HitInfo hitInfo)
    {
        float damage = CalculateDamage(damageContext);
        hitInfo.damage = damage;
        target.TakeDamage(hitInfo);
    }

    public float CalculateDamage(DamageContext damageContext)
    {
        float result = _stats.strength;

        result += damageContext.baseDamage;
        result += damageContext.attackDamage;
        result *= damageContext.attackMultiplier;

        return result;
    }

    public void PostAttack()
    {
        _stateMachine.ChangeState(_normalState);
    }

    public void TakeDamage(HitInfo hitInfo)
    {
        if (_stateMachine._activeIFrame) return;

        Debug.Log(gameObject.name + " Take Damage. Causer is " + hitInfo.causer.name);
        DecreaseHp(hitInfo.damage);
        _stateMachine.OnHit(hitInfo);
    }

    public void IncreaseHp(float increaseValue)
    {
        _currentHp = Mathf.Clamp(_currentHp + increaseValue, 0, _stats.maxHp);
        Debug.Log("Increase Hp. Current Hp is " + _currentHp);
    }

    public void DecreaseHp(float decreaseValue)
    {
        _currentHp = Mathf.Clamp(_currentHp - decreaseValue, 0, _stats.maxHp);
        Debug.Log("Decrease Hp. Current Hp is " + _currentHp);

        if (_currentHp == 0) _stateMachine.ChangeState(_dieState);
    }

    public void PostHit()
    {
        _stateMachine.ChangeState(_normalState);
    }

    protected void BindInputAction()
    {
        var actions = InputReader.Instance.inputActions.PlayerActionMap;

        InputReader.Instance.BindAction(actions.Move,
            performed: () => _stateMachine.OnMoveInput(actions.Move.ReadValue<Vector2>()),
            canceled: () => _stateMachine.OnEndMoveInput());

        InputReader.Instance.BindAction(actions.Jump,
            started: () => _stateMachine.OnJumpInput(),
            canceled: () => _stateMachine.OnEndJumpInput());

        InputReader.Instance.BindAction(actions.Melee,
            started: () => _stateMachine.OnAttackInput(AttackType.Melee),
            canceled: () => _stateMachine.OnEndAttackInput());

        InputReader.Instance.BindAction(actions.Ranged,
            started: () => _stateMachine.OnAttackInput(AttackType.Range),
            canceled: () => _stateMachine.OnEndAttackInput());

        InputReader.Instance.BindAction(actions.Skill,
            started: () => _stateMachine.OnAttackInput(AttackType.Skill),
            canceled: () => _stateMachine.OnEndAttackInput());
    }

    private void OnDisable()
    {
        
    }
}