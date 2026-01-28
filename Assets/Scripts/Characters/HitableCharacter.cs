using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitableCharacter : CharacterBase, IHitable
{
    [SerializeField] protected HitDatabase _hitDatabase;

    public HitDatabase HitData => _hitDatabase;

    public void TakeDamage(HitInfo hitInfo)
    {
        if (_stateMachine._activeIFrame) return;

        Debug.Log(gameObject.name + " Take Damage. Causer is " + hitInfo.causer.name);
        DecreaseHp(hitInfo.damage);
        _stateMachine.OnHit(hitInfo);
    }

    public void IncreaseHp(float increaseValue)
    {
        _currentHp = Mathf.Clamp(_currentHp + increaseValue, 0, _stats.maxHp);
        Debug.Log("Increase Hp. Current Hp is " + _currentHp);
    }

    public void DecreaseHp(float decreaseValue)
    {
        _currentHp = Mathf.Clamp(_currentHp - decreaseValue, 0, _stats.maxHp);
        Debug.Log("Decrease Hp. Current Hp is " + _currentHp);

        if (_currentHp == 0) _stateMachine.ChangeState(_stateMachine._dieState);
    }

    protected override void Die()
    {

    }
}
