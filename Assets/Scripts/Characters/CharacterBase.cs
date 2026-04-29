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
    HitDatabase HitData { get; }

    void TakeDamage(HitInfo hitInfo);
    void IncreaseHp(float increaseValue);
    void DecreaseHp(float decreaseValue);
    void PostHit();
}

public interface IAttackable
{  
    AttackState _attackState { get; }

    void ApplyDamage(IHitable target, DamageContext damageContext, HitInfo hitInfo);
    float CalculateDamage(DamageContext damageContext);
    void PostAttack(float recovery);
}

public interface IStunable { public StunState _stunState { get; } }
public interface IGroggyable { public GroggyState _groggyState { get; } }

public interface ICarryable 
{ 
    public CarryState _carryState { get; } 
    public Transform CarryHoldSocket { get; }
}

public interface IPushable { public PushState _pushState { get; } }

public interface ILadderable { public LadderState _ladderState { get; } }

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
[RequireComponent(typeof(MovementComponent2D), typeof(CharacterStateMachine))]
public abstract class CharacterBase : MonoBehaviour
{
    [SerializeField] protected LayerMask _targetLayer;

    [Header("Data Asset")]
    //[SerializeField] protected CharacterStats _stats;
    [SerializeReference, SubclassSelector] protected CharacterStatsBase _stats;

    [Header("Children Object")]
    [SerializeField] protected GameObject _spriteObject;

    public Rigidbody2D _rigidBody { get; private set; }
    public CapsuleCollider2D _mainCollider { get; private set; }
    public MovementComponent2D _movement { get; private set; }
    public CharacterStateMachine _stateMachine { get; private set; }
    public RoomManager _currentRoom { get; private set; }

    protected float _currentHp;
    protected int _maxJumpCount = 1;
    protected int _currentJumpCount = 0;

    public NormalState _normalState { get; private set; }
    public DieState _dieState { get; private set; }

    public CharacterStatsBase Stats => _stats;
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
        _movement.Initialize(_stats, _spriteObject);
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

    public void SetCurrentRoom(RoomManager roomManager)
    {
        _currentRoom = roomManager;

        if (TryGetComponent<AINavigator>(out var navigator))
        {
            navigator.SetCurrentRoom(roomManager);
        }
    }

    public void SetActiveCharacter(Vector2 spawnPos)
    {
        gameObject.SetActive(true);

        StartCoroutine(DelayedInit(spawnPos));
    }

    private IEnumerator DelayedInit(Vector2 spawnPos)
    {
        yield return null;

        PostEnabled(spawnPos);
    }

    protected virtual void PostEnabled(Vector2 spawnPos)
    {
        if (_rigidBody.IsSleeping())
        {
            _rigidBody.WakeUp();
        }

        gameObject.transform.position = spawnPos;
        //_stateMachine.ChangeState(_normalState);
        //ResetState();
    }

    public virtual void SetDeactiveCharacter()
    {
        gameObject.SetActive(false);
    }

    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }

    public void InstantKill()
    {
        SetCurrentHp(0);
        _stateMachine.ChangeState(_dieState);
    }

    public abstract void ResetState();

    protected void SetCurrentHp(float hp)
    {
        _currentHp = Mathf.Clamp(hp, 0, _stats.maxHp);
    }

    protected abstract void Die();
}