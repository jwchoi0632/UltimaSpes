using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class ProjectileBase : MonoBehaviour, IPoolable<ProjectileBase>
{
    [Header("Projectile Settings")]
    [SerializeField] protected bool _isOverlapEvent = true;
    [SerializeField] protected float _gravity = 0.0f;
    [SerializeField] protected bool _isPiercing = false;

    protected Rigidbody2D _rb;
    protected BoxCollider2D _collider;
    protected GameObject _owner;

    protected Coroutine _deactivateCoroutine;
    protected LayerMask _targetLayer;
    protected HitInfo _hitInfo;
    protected DamageContext _damageContext;
    protected AttackDataBase _attackData;

    protected float _activeTime;

    public Action<ProjectileBase> OnReturnToPool { get; set; }
    
    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();

        _rb.gravityScale = _gravity;
        _collider.isTrigger = _isOverlapEvent;
    }

    public void SetLifeTime(float timeValue)
    {
        _activeTime = timeValue;
    }

    public virtual void Init(AttackDataBase attackData, LayerMask layer)
    {
        _attackData = attackData;
        _owner = _attackData.attackInfo.causer;
        _hitInfo = _attackData.attackInfo;
        _targetLayer = layer;
        
    }

    public virtual void Init(GameObject owner, HitInfo hitInfo, LayerMask layer)
    {
        _owner = owner;
        _hitInfo = hitInfo;
        _targetLayer = layer;
    }

    public void Launch(AttackContext attackContext, float force)
    {
        gameObject.SetActive(true);

        if (_rb.IsSleeping())
        {
            _rb.WakeUp();
        }

        _damageContext = attackContext.damageContext;

        if (_deactivateCoroutine != null) StopCoroutine(_deactivateCoroutine);

        _deactivateCoroutine = StartCoroutine(DeactivateTimer());

        float angle = Mathf.Atan2(attackContext.direction.y, attackContext.direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        transform.position = attackContext.spawnPos;

        _rb.velocity = attackContext.direction * force;
    }

    private void OnTriggerEnter2D(Collider2D collision) => OnHitTarget(collision.gameObject);
    private void OnCollisionEnter2D(Collision2D collision) => OnHitTarget(collision.gameObject);

    protected virtual void OnHitTarget(GameObject target)
    {
        if (target == _owner) return;

        if (((1 << target.layer) & _attackData.obstacleLayer) != 0)
        {
            gameObject.SetActive(false);
        }

        if (((1 << target.layer) & _targetLayer) != 0)
        {
            if (((1 << target.layer) & _attackData.breakableLayer) != 0)
            {
                if (!_attackData.breakable && !_isPiercing)
                {
                    gameObject.SetActive(false);
                    return;
                }
            }

            if (target.TryGetComponent<IHitable>(out IHitable hitable))
            {
                if (_owner.TryGetComponent<IAttackable>(out var attacker))
                {
                    attacker.ApplyDamage(hitable, _damageContext, _hitInfo);
                }
            }

            if (!_isPiercing) gameObject.SetActive(false);
        }
    }

    protected IEnumerator DeactivateTimer()
    {
        yield return new WaitForSeconds(_activeTime);

        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (_rb != null)
        {
            _rb.velocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            _rb.Sleep();
        }

        if (OnReturnToPool != null && OnReturnToPool.Target != null)
        {
            OnReturnToPool.Invoke(this);
        }
    }

    private void OnDestroy()
    {
        OnReturnToPool = null;
    }
}