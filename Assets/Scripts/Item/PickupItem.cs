using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D), typeof(Rigidbody2D))]
public class PickupItem : MonoBehaviour, IInteractable, IPoolable<PickupItem>
{
    [SerializeField] protected InteractionType _interactionType;
    [SerializeField] protected float _moveSpeed = 8.0f;
    [SerializeField] protected LayerMask _groundLayer;

    protected CircleCollider2D _col;
    protected Rigidbody2D _rb;
    protected GameObject _causer;

    public InteractionType SupportedType => _interactionType;

    public System.Action<PickupItem> OnReturnToPool { get; set; }

    protected bool _isPickedup = false;

    public void InitItem(int _id)
    {
        // TODO : id에 따라 sprite 및 데이터 적용
    }

    public bool CanInteraction(GameObject causer, InteractionType type)
    {
        return !_isPickedup;
    }

    public void OnInteraction(GameObject causer, InteractionType type, Vector2 force = default)
    {
        if (_isPickedup) return;

        _causer = causer;
        _isPickedup = true;

        _rb.simulated = false;
        _col.isTrigger = true;

        StartCoroutine(MoveToCauserRoutine());
    }

    public void Drop(Vector3 startPos)
    {
        gameObject.SetActive(true);

        if (_rb.IsSleeping())
        {
            _rb.WakeUp();
        }

        _isPickedup = false;
        _causer = null;
        _rb.simulated = true;
        _col.isTrigger = false;
        _rb.constraints = RigidbodyConstraints2D.None;

        _rb.velocity = Vector2.zero;
        transform.position = startPos;

        Vector2 spawnForce = new Vector2(Random.Range(-2f, 2f), 5f);
        _rb.AddForce(spawnForce, ForceMode2D.Impulse);
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<CircleCollider2D>();
        _col.isTrigger = false;
    }

    private IEnumerator MoveToCauserRoutine()
    {
        while (_causer != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                _causer.transform.position,
                _moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, _causer.transform.position) < 0.1f)
            {
                gameObject.SetActive(false);
                yield break;
            }
            yield return null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & _groundLayer) != 0)
        {
            _rb.velocity = Vector2.zero;
            _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    private void OnDisable()
    {
        if (_rb != null)
        {
            _rb.velocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            _rb.Sleep();
        }

        OnReturnToPool?.Invoke(this);
    }

    private void OnDestroy()
    {
        OnReturnToPool = null;
    }
}