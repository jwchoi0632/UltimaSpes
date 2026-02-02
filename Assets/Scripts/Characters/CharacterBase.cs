using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct HitInfo
{
    public GameObject causer;
    public float damage;
    public HitType hitType;
    public float hitDuration;
    public bool isStun;
    public float stunDuration;
    public float knockForce;
    public float launchForce;
}

public struct DamageContext
{
    public float baseDamage;
    public float attackMultiplier;
    public float attackDamage;
}

public enum AttackType
{
    Melee,
    Range,
    Skill
}

public interface IHitable 
{ 
    HitState _hitState { get; }
    void TakeDamage(HitInfo hitInfo);
    void IncreaseHp(float increaseValue);
    void DecreaseHp(float decreaseValue);
}

public interface IAttackable
{  
    AttackState _attackState { get; }

    void ApplyDamage(IHitable target, DamageContext damageContext, HitInfo hitInfo);
    float CalculateDamage(DamageContext damageContext);
}

public interface IStunable { public StunState _stunState { get; } }
public interface IGroggyable { public GroggyState _groggyState { get; } }

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
[RequireComponent(typeof(MovementComponent2D), typeof(CharacterStateMachine))]
public abstract class CharacterBase : MonoBehaviour
{
    [SerializeField] protected LayerMask _targetLayer;

    [Header("Data Asset")]
    [SerializeField] protected CharacterStats _stats;

    [Header("Children Object")]
    [SerializeField] protected GameObject _spriteObject;

    public Rigidbody2D _rigidBody { get; private set; }
    public CapsuleCollider2D _mainCollider { get; private set; }
    public MovementComponent2D _movement { get; private set; }
    public CharacterStateMachine _stateMachine { get; private set; }

    protected float _currentHp;
    protected int _maxJumpCount = 1;
    protected int _currentJumpCount = 0;

    public NormalState _normalState { get; private set; }
    public DieState _dieState { get; private set; }

    public CharacterStats Stats => _stats;
    public GameObject Sprite => _spriteObject;
    public LayerMask TargetLayer => _targetLayer;

    private void Awake()
    {
        InitComponents();
        InitState();
        OnAwake();
    }

    void Start()
    {
        _currentHp = _stats.maxHp;
        OnStart();
    }

    protected virtual void InitComponents()
    {
        _movement = GetComponent<MovementComponent2D>();
        _stateMachine = GetComponent<CharacterStateMachine>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _mainCollider = GetComponent<CapsuleCollider2D>();
    }

    protected virtual void InitState()
    {
        _normalState = new NormalState(this);
        _dieState = new DieState(this);
    }

    public virtual void SetActiveCharacter()
    {
        _stateMachine.ChangeState(_normalState);
    }

    public virtual void SetDeactiveCharacter()
    {

    }

    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }

    public void InstantKill()
    {
        SetCurrentHp(0);
        _stateMachine.ChangeState(_dieState);
    }

    protected void SetCurrentHp(float hp)
    {
        _currentHp = Mathf.Clamp(hp, 0, _stats.maxHp);
    }

    protected abstract void Die();
}
