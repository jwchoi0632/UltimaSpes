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
[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
[RequireComponent(typeof(MovementComponent2D))]
public abstract class CharacterBase : MonoBehaviour
{
    [Header("Data Asset")]
    [SerializeField] protected CharacterStats stats;

    protected Rigidbody2D rigidBody;
    protected CapsuleCollider2D mainCollider;
    protected SpriteRenderer mainRenderer;
    protected Animator animator;
    protected MovementComponent2D movement;

    protected CharacterState currentState;
    protected float currentHp;
    protected int maxJumpCount = 1;
    protected int currentJumpCount = 0;

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
        rigidBody = GetComponent<Rigidbody2D>();
        mainCollider = GetComponent<CapsuleCollider2D>();
        mainRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        movement = GetComponent<MovementComponent2D>();
        movement.Init(mainCollider, rigidBody, stats);
    }

    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }

    public void ChangeState (CharacterState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
    }

    protected abstract void Die();
}
