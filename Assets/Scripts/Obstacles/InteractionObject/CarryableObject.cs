using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class CarryableObject : MonoBehaviour, IInteractable
{
    [SerializeField] protected InteractionType _interactiontype;

    protected Rigidbody2D _rb;
    protected BoxCollider2D _col;

    protected bool _isOnCarry = false;

    public InteractionType SupportedType => _interactiontype;

    public bool CanInteraction(GameObject causer, InteractionType type)
    {
        if (type == InteractionType.Carry) return !_isOnCarry;
        else if (type == InteractionType.Throw) return _isOnCarry;

        return false;
    }

    public void OnInteraction(GameObject causer, InteractionType type, Vector2 force = default)
    {
        if (type == InteractionType.Carry) OnCarry(causer);
        else if (type == InteractionType.Throw) OnThrow(causer, force);
    }

    protected virtual void OnCarry(GameObject causer)
    {
        _isOnCarry = true;

        _rb.velocity = Vector2.zero;
        _rb.angularVelocity = 0f;

        _rb.simulated = false;
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _col.enabled = false;
    }

    protected virtual void OnThrow(GameObject causer, Vector2 force)
    {
        _isOnCarry = false;
        _col.enabled = true;
        _rb.simulated = true;
        _rb.bodyType = RigidbodyType2D.Dynamic;

        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        ResolveOverlap(causer);

        _rb.AddForce(force, ForceMode2D.Impulse);

        StopAllCoroutines();
        StartCoroutine(LockPositionOnLanding());
    }

    private void ResolveOverlap(GameObject causer)
    {
        if (causer.TryGetComponent<Collider2D>(out var causerCol))
        {
            if (_col.bounds.Intersects(causerCol.bounds))
            {
                float overlapY = causerCol.bounds.max.y - _col.bounds.min.y;

                if (overlapY > 0)
                {
                    transform.position += new Vector3(0, overlapY + 0.01f, 0);

                    //Physics2D.SyncTransforms();
                }
            }
        }
    }

    private IEnumerator LockPositionOnLanding()
    {
        yield return new WaitForSeconds(0.2f);

        yield return new WaitUntil(() => _rb.velocity.sqrMagnitude < 0.01f);

        _rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        //_rb.velocity = new Vector2(0, _rb.velocity.y);
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}