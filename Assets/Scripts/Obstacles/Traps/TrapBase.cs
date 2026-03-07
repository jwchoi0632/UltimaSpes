using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class TrapBase : MonoBehaviour, IAttackable
{
    [Header("Trap Option")]
    [SerializeField] protected HitInfo _hitInfo;
    [SerializeField] protected float _coolDown = 0.0f;
    [SerializeField] protected bool _isOneTimeOnly = false;
    [SerializeField] protected bool _isBlockingTrap = false;

    protected bool _isReady = true;
    protected BoxCollider2D _collider;

    public AttackState _attackState { get; private set; }

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _collider.isTrigger = !_isBlockingTrap;
        _hitInfo.causer = gameObject;

        if (_isBlockingTrap) gameObject.layer = LayerMask.NameToLayer("Ground");
    }

    public void Activate(GameObject target)
    {
        if (!_isReady) return;

        _isReady = false;

        OnActivate(target);

        if (!_isOneTimeOnly) StartCoroutine(ApplyCoolDown());
    }

    public void ApplyDamage(IHitable target, DamageContext damageContext, HitInfo hitInfo)
    {
        hitInfo.damage = CalculateDamage(damageContext);
        target?.TakeDamage(hitInfo);
    }
    public float CalculateDamage(DamageContext damageContext)
    {
        float result = _hitInfo.damage;

        result += damageContext.baseDamage;
        result += damageContext.attackDamage;
        result *= damageContext.attackMultiplier;

        return result;
    }

    public void PostAttack(float recovery) { }

    protected abstract void OnActivate(GameObject Target);

    protected IEnumerator ApplyCoolDown()
    {
        yield return new WaitForSeconds(_coolDown);

        _isReady = true;
    }
}