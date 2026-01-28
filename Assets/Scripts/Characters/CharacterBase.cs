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

public interface IHitable 
{ 
    public void TakeDamage(HitInfo hitInfo);
    public void IncreaseHp(float increaseValue);
    public void DecreaseHp(float decreaseValue);
}

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
[RequireComponent(typeof(MovementComponent2D), typeof(CharacterStateMachine))]
public abstract class CharacterBase : MonoBehaviour
{
    [Header("Data Asset")]
    [SerializeField] protected CharacterStats _stats;

    [Header("Children Object")]
    [SerializeField] protected GameObject _spriteObject;

    public Rigidbody2D _rigidBody { get; private set; }
    public CapsuleCollider2D _mainCollider { get; private set; }
    protected MovementComponent2D _movement;
    protected CharacterStateMachine _stateMachine;

    protected float _currentHp;
    protected int _maxJumpCount = 1;
    protected int _currentJumpCount = 0;

    public CharacterStats Stats => _stats;
    public GameObject Sprite => _spriteObject;

    private void Awake()
    {
        InitComponents();
        OnAwake();
    }

    void Start()
    {
        _currentHp = _stats.maxHp;
        OnStart();
    }

    private void InitComponents()
    {
        _movement = GetComponent<MovementComponent2D>();
        _stateMachine = GetComponent<CharacterStateMachine>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _mainCollider = GetComponent<CapsuleCollider2D>();
    }

    public virtual void SetActiveCharacter()
    {
        _stateMachine.ChangeState(_stateMachine._normalState);
    }

    public virtual void SetDeactiveCharacter()
    {

    }

    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }

    public void InstantKill()
    {
        SetCurrentHp(0);
        _stateMachine.ChangeState(_stateMachine._dieState);
    }

    public void ApplyDamage(IHitable target)
    {
        HitInfo hitInfo = new HitInfo();

        hitInfo.damage = CalculateDamage();
        hitInfo.causer = gameObject;

        target.TakeDamage(hitInfo);
    }

    protected void SetCurrentHp(float hp)
    {
        _currentHp = Mathf.Clamp(hp, 0, _stats.maxHp);
    }

    protected float CalculateDamage()
    {
        float result = _stats.strength;

        return result;
    }

    protected abstract void Die();
}
