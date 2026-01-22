using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class ProjectileBase : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] protected bool _isOverlapEvent = true;
    [SerializeField] protected float _activeTime = 3.0f;
    [SerializeField] protected float _gravity = 0.0f;

    protected Rigidbody2D _rb;
    protected CircleCollider2D _collider;
    protected GameObject _owner;

    protected Coroutine _deactivateCoroutine;
    protected LayerMask _targetLayer;
    protected float _damage;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CircleCollider2D>();

        _rb.gravityScale = _gravity;
        _collider.isTrigger = _isOverlapEvent;
    }

    public virtual void Init(GameObject owner, float damage, LayerMask layer)
    {
        _owner = owner;
        _damage = damage;
        _targetLayer = layer;
    }

    public void Launch(Vector2 direction, float force)
    {
        gameObject.SetActive(true);

        if (_rb.IsSleeping())
        {
            _rb.WakeUp();
        }

        if (_deactivateCoroutine != null) StopCoroutine(_deactivateCoroutine);

        _deactivateCoroutine = StartCoroutine(DeactivateTimer());

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        _rb.velocity = direction * force;
    }

    private void OnTriggerEnter2D(Collider2D collision) => OnHitTarget(collision.gameObject);
    private void OnCollisionEnter2D(Collision2D collision) => OnHitTarget(collision.gameObject);

    protected virtual void OnHitTarget(GameObject target)
    {
        if (target == _owner) return;

        if (((1 << target.layer) & _targetLayer) != 0)
        {
            CharacterBase character = target.GetComponent<CharacterBase>();

            character?.DecreaseHp(_damage);
            gameObject.SetActive(false);
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
    }
}