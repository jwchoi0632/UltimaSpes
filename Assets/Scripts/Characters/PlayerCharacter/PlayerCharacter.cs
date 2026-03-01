using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeaponComponent), typeof(InteractionComponent), typeof(PlayerController))]
public class PlayerCharacter : CharacterBase, IAttackable, IStunable, IGroggyable, IHitable, ICarryable, IPushable
{
    [SerializeField] protected HitDatabase _hitDatabase;
    [SerializeField] private Transform _carryHoldSocket;

    public AttackState _attackState { get; private set; }
    public StunState _stunState { get; private set; }
    public GroggyState _groggyState { get; private set; }
    public HitState _hitState { get; private set; }
    public CarryState _carryState { get; private set; }
    public PushState _pushState { get; private set; }

    public PlayerController _playerControlelr { get; private set; }
    public WeaponComponent _weaponComponent { get; private set; }
    public InteractionComponent _interactionComponent { get; private set; }

    public HitDatabase HitData => _hitDatabase;
    public Transform CarryHoldSocket => _carryHoldSocket;

    protected override void OnAwake()
    {
        base.OnAwake();
        
        _playerControlelr = GetComponent<PlayerController>();
        _weaponComponent = GetComponent<WeaponComponent>();
        _interactionComponent = GetComponent<InteractionComponent>();
    }

    protected override void OnStart()
    {
        base.OnStart();

        _playerControlelr.BindInputAction();
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
        _carryState = new CarryState(this);
        _pushState = new PushState(this);
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
        ResetState();
    }

    public void TakeDamage(HitInfo hitInfo)
    {
        if (_stateMachine._activeIFrame) return;

        Debug.Log(gameObject.name + " Take Damage. Causer is " + hitInfo.causer?.name);
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
        ResetState();
    }

    public override void ResetState()
    {
        _stateMachine.ChangeState(_normalState);
    }

    private void OnDisable()
    {
        
    }
}