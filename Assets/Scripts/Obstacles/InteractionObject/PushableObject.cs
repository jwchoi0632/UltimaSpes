using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PushableObject : MonoBehaviour, IInteractable
{
    [SerializeField] InteractionType _interactionType;

    private Rigidbody2D _rb;
    private Rigidbody2D _causerRb;
    private BoxCollider2D _col;
    private bool _isBeingPushed = false;

    public InteractionType SupportedType => _interactionType;

    public bool CanInteraction(GameObject causer, InteractionType type)
    {
        return true;
    }

    public void OnInteraction(GameObject causer, InteractionType type, Vector2 force = default)
    {
        if (type == InteractionType.Push)
        {
            _isBeingPushed = true;
            causer.TryGetComponent<Rigidbody2D>(out _causerRb);
            _rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }
        else if (type == InteractionType.None)
        {
            _isBeingPushed = false;
            _causerRb = null;
            _rb.velocity = new Vector2(0, _rb.velocity.y);
            _rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        _rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
    }

    private void FixedUpdate()
    {
        if (_isBeingPushed && _causerRb != null)
        {
            _rb.velocity = new Vector2(_causerRb.velocity.x, _rb.velocity.y);
        }
    }
}