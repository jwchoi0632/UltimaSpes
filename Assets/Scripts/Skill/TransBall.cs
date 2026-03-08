using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class TransBall : MonoBehaviour, IPoolable<TransBall>
{
    public Action<TransBall> OnReturnToPool { get; set; }

    [SerializeField] private float _activeSpeed = 1.0f;

    private CircleCollider2D _collider;
    private TransBall _exitPos;
    private Vector2 _scale;
    private LayerMask _transTargetLayer;
    private bool isActive;

    public void SetTransTargetLayer(LayerMask targetLayer) => _transTargetLayer = targetLayer;
    public void SetExitPos(TransBall pos) => _exitPos = pos;

    public void Activate()
    {
        _scale = Vector2.zero;
        transform.localScale = _scale;

        gameObject.SetActive(true);
        isActive = true;
        _collider.enabled = true;
    }

    public void Deactivate()
    {
        _scale = Vector2.one;
        transform.localScale = _scale;

        isActive = false;
        _collider.enabled = false;
    }

    private void Awake()
    {
        _collider = GetComponent<CircleCollider2D>();
        _collider.isTrigger = true;
    }

    private void Update()
    {
        if (isActive && transform.localScale.x < 1)
        {
            _scale.x += _activeSpeed * Time.deltaTime;
            _scale.y += _activeSpeed * Time.deltaTime;
            transform.localScale = _scale;
            return;
        }
        
        if (!isActive)
        {
            if (gameObject.transform.localScale.x > 0)
            {
                _scale.x -= _activeSpeed * Time.deltaTime;
                _scale.y -= _activeSpeed * Time.deltaTime;
                transform.localScale = _scale;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_exitPos == null) return;

        if (((1 << collision.gameObject.layer) & _transTargetLayer) != 0)
        {
            Debug.Log(collision.gameObject.name);
            Teleport(collision);
        }
    }

    private void Teleport(Collider2D collision)
    {
        collision.transform.position = _exitPos.transform.position;

        _exitPos.Deactivate();
        Deactivate();
    }

    private void OnDisable()
    {
        OnReturnToPool?.Invoke(this);
    }

    private void OnDestroy()
    {
        OnReturnToPool = null;
    }
}