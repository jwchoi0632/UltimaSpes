using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitableEnemyCharacter : EnemyCharacterBase, IHitable, IStunable, IGroggyable
{
    [SerializeField] protected HitDatabase _hitDatabase;

    public HitState _hitState { get; private set; }
    public StunState _stunState { get; private set; }
    public GroggyState _groggyState { get; private set; }

    public HitDatabase HitData => _hitDatabase;

    public void DecreaseHp(float decreaseValue)
    {
        _currentHp = Mathf.Clamp(_currentHp - decreaseValue, 0, _stats.maxHp);
        Debug.Log(gameObject.name + "Decrease Hp. Current Hp is " + _currentHp);

        if (_currentHp == 0) _stateMachine.ChangeState(_dieState);
    }

    public void IncreaseHp(float increaseValue)
    {
        _currentHp = Mathf.Clamp(_currentHp + increaseValue, 0, _stats.maxHp);
        Debug.Log(gameObject.name + "Increase Hp. Current Hp is " + _currentHp);
    }

    public void TakeDamage(HitInfo hitInfo)
    {
        if (_stateMachine._activeIFrame) return;

        Debug.Log(gameObject.name + " Take Damage. Causer is " + hitInfo.causer.name);
        DecreaseHp(hitInfo.damage);
        _stateMachine.OnHit(hitInfo);

        if (this is IChaseable chaseable)
        {
            if ((_enemyStats?.targetLayer.value & (1 << hitInfo.causer.layer)) != 0)
            {
                chaseable._chaseState.SetChaseTarget(hitInfo.causer.transform);
            }
        }
    }

    public void PostHit()
    {
        if (_stateMachine._currentState == _dieState) return;

        if (_controller is EnemyAIController enemyController)
        {
            enemyController.RequestPostHitAction();
        }
    }

    protected override void InitState()
    {
        base.InitState();

        _hitState = new HitState(this);
        _stunState = new StunState(this);
        _groggyState = new GroggyState(this);
    }
}