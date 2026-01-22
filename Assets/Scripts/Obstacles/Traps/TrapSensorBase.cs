using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TrapSensorBase : MonoBehaviour
{
    [Header("Link")]
    [SerializeField] protected TrapBase _linkedTrap;

    [Header("Physics")]
    [SerializeField] protected bool _isOverlapEvent = true;

    [Header("Detection")]
    [SerializeField] protected LayerMask _targetLayer;

    protected BoxCollider2D _sensor;

    private void Awake()
    {
        _sensor = GetComponent<BoxCollider2D>();

        _sensor.isTrigger = _isOverlapEvent;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ProcessDetection(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ProcessDetection(collision.gameObject);
    }

    private bool IsTargetObject(GameObject target)
    {
        if (_linkedTrap == null) return false;

        return ((1 << target.layer) & _targetLayer) != 0;
    }

    private void ProcessDetection(GameObject target)
    {
        if (!IsTargetObject(target)) return;

        if (!CheckCondition(target)) return;

        _linkedTrap.Activate(target);
    }

    protected virtual bool CheckCondition(GameObject target)
    {
        return true;
    }
}