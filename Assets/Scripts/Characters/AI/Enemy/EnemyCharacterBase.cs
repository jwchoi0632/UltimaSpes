using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PerceptionComponent), typeof(AINavigator), typeof(WeaponComponent))]
public class EnemyCharacterBase : AICharacterBase, IPoolable<EnemyCharacterBase>, IAttackable
{
    public PerceptionComponent _perception { get; private set; }
    public AINavigator _navigation { get; private set; }
    public WeaponComponent _weapon { get; private set; }

    public AttackState _attackState { get; private set; }

    protected EnemyStats _enemyStats;


    public Action<EnemyCharacterBase> OnReturnToPool { get; set; }

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

    public void PostAttack(float recovery)
    {
        if (recovery > 0.05f)
        {
            if (this is IChaseable chasesable)
            {
                _stateMachine.OnWait(recovery, chasesable._chaseState);
                return;
            }
        }
        
        ResetState();
    }

    protected override void Die()
    {
        Debug.Log(gameObject.name + " die");
        _stateMachine.ChangeState(_dieState);
    }

    protected override void InitComponents()
    {
        base.InitComponents();

        _perception = GetComponent<PerceptionComponent>();
        _navigation = GetComponent<AINavigator>();
        _weapon = GetComponent<WeaponComponent>();

        _enemyStats = _stats as EnemyStats;
    }

    protected override void InitState()
    {
        base.InitState();

        _attackState = new AttackState(this);
    }

    protected override void OnStart()
    {
        base.OnStart();

        SetActiveCharacter(transform.position); // test
    }

    public override void ResetState()
    {
        _stateMachine.OnPatrol();
    }

    private void OnDisable()
    {
        if (_rigidBody != null)
        {
            _rigidBody.velocity = Vector2.zero;
            _rigidBody.angularVelocity = 0f;
            _rigidBody.Sleep();
        }

        if (OnReturnToPool != null && OnReturnToPool.Target != null)
        {
            OnReturnToPool.Invoke(this);
        }
    }
}