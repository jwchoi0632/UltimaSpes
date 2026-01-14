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
//[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public abstract class CharacterBase : MonoBehaviour
{
    [Header("Data Asset")]
    [SerializeField] protected CharacterStats stats;

    protected Rigidbody2D rigidBody;
    protected CapsuleCollider2D mainCollider;
    protected SpriteRenderer mainRenderer;
    protected Animator animator;

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
    }

    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }

    public void ChangeState (CharacterState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
    }

    protected virtual void Move(Vector2 direction)
    {
        rigidBody.velocity = new Vector2(direction.x * stats.moveSpeed, rigidBody.velocity.y);
    }

    protected virtual void StartJump()
    {
        if (CheckGrounded() || 
            (!CheckGrounded() && currentJumpCount < maxJumpCount))
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, stats.jumpForce);
            currentJumpCount++;
        }
    }

    protected virtual void ContinuingJump(float deltaTime)
    {
        if (rigidBody.velocity.y > 0)
        {
            rigidBody.velocity += Vector2.up * (stats.jumpForce * 0.5f * deltaTime);
        }
    }

    protected bool CheckGrounded()
    {
        bool result = true;

        // TODO : 지면 체크

        return result;
    }

    protected abstract void Die();
}
