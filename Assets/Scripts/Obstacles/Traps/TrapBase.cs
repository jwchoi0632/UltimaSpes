using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TrapBase : MonoBehaviour
{
    [Header("Trap Option")]
    [SerializeField] protected float _damage = 10.0f;
    [SerializeField] protected float _coolDown = 0.0f;
    [SerializeField] protected bool _isOneTimeOnly = false;

    protected bool _isReady = true;

    public void Activate(GameObject target)
    {
        if (!_isReady) return;

        _isReady = false;

        OnActivate(target);

        if (!_isOneTimeOnly) StartCoroutine(ApplyCoolDown());
    }

    protected abstract void OnActivate(GameObject Target);

    protected IEnumerator ApplyCoolDown()
    {
        yield return new WaitForSeconds(_coolDown);

        _isReady = true;
    }
}