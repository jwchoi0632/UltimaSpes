using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitableCharacter : CharacterBase, IHitable
{
    public void TakeDamage(HitInfo hitInfo)
    {
        Debug.Log(gameObject.name + " Take Damage. Causer is " + hitInfo.causer.name);
        DecreaseHp(hitInfo.damage);
        _stateMachine.OnHit(hitInfo);
    }

    protected void IncreaseHp(float increaseValue)
    {
        _currentHp = Mathf.Clamp(_currentHp + increaseValue, 0, _stats.maxHp);
        Debug.Log("Increase Hp. Current Hp is " + _currentHp);
    }

    protected void DecreaseHp(float decreaseValue)
    {
        _currentHp = Mathf.Clamp(_currentHp - decreaseValue, 0, _stats.maxHp);
        Debug.Log("Decrease Hp. Current Hp is " + _currentHp);

        if (_currentHp == 0) _stateMachine.ChangeState(_stateMachine._dieState);
    }

    protected override void Die()
    {

    }
}
