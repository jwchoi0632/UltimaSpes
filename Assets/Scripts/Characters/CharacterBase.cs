using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterState
{
    Idle,
    Move,
    Falling,
    Jumping
}

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
[RequireComponent(typeof(MovementComponent2D))]
public abstract class CharacterBase : MonoBehaviour
{
    [Header("Data Asset")]
    [SerializeField] protected CharacterStats stats;

    [Header("Children Object")]
    [SerializeField] protected GameObject spriteObject;

    public Rigidbody2D rigidBody { get; private set; }
    public CapsuleCollider2D mainCollider { get; private set; }
    protected MovementComponent2D movement;

    protected CharacterState currentState;
    protected float currentHp;
    protected int maxJumpCount = 1;
    protected int currentJumpCount = 0;

    public CharacterStats Stats => stats;
    public GameObject Sprite => spriteObject;

    private void Awake()
    {
        InitComponents();
        OnAwake();
    }

    void Start()
    {
        currentHp = stats.maxHp;
        currentState = CharacterState.Idle;
        OnStart();
    }

    void Update()
    {
        
    }

    private void InitComponents()
    {
        movement = GetComponent<MovementComponent2D>();
        rigidBody = GetComponent<Rigidbody2D>();
        mainCollider = GetComponent<CapsuleCollider2D>();
    }

    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }

    public void ChangeState (CharacterState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
    }

    public void SetCurrentHp(float hp)
    {
        currentHp = Mathf.Clamp(hp, 0, stats.maxHp);
    }

    public void IncreaseHp(float increaseValue)
    {
        currentHp = Mathf.Clamp(currentHp + increaseValue, 0, stats.maxHp);
        Debug.Log("Increase Hp. Current Hp is " + currentHp);
    }
    public void DecreaseHp(float decreaseValue)
    {
        currentHp = Mathf.Clamp(currentHp - decreaseValue, 0, stats.maxHp);
        Debug.Log("Decrease Hp. Current Hp is " + currentHp);

        if (currentHp == 0) Die();
    }

    public void InstantKill()
    {
        SetCurrentHp(0);
        Die();
    }

    protected abstract void Die();
}
