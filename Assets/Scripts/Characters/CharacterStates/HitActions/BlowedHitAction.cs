using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlowedHitAction", menuName = "Combat/HitActions/BlowedHit")]
public class BlowedHitAction : HitActionBase
{
    private float _startPos;
    private bool _isCrashed;

    private System.Action<Collision2D> _collisionHandler;

    public override void OnStart(CharacterStateMachine stateMachine)
    {
        Debug.Log("On Blowed");
        _isCrashed = false;
        _startPos = stateMachine._character.transform.position.x;

        _collisionHandler += (collision) => OnCrashed(stateMachine, collision);
        stateMachine.OnCollisionEntered += _collisionHandler;
    }

    public override void OnUpdate(CharacterStateMachine stateMachine, HitInfo info)
    {
        if (_isCrashed) return;

        if (stateMachine._movement._rb.velocity.x < 0.1f &&
            stateMachine._movement._rb.velocity.x > -0.1f)
        {
            stateMachine.ChangeState(stateMachine._normalState);
        }
    }

    private void OnCrashed(CharacterStateMachine stateMachine, Collision2D collision)
    {
        _isCrashed = true;

        GameObject otherObj = collision.gameObject;
        GameObject ownerObj = stateMachine.gameObject;

        float distance = Mathf.Abs(ownerObj.transform.position.x - _startPos);

        if (distance > 1.0f)
        {
            float finalDamage = distance;

            if (otherObj.TryGetComponent<IHitable>(out var other))
            {
                other.TakeDamage(new HitInfo { causer = ownerObj, 
                                                damage = finalDamage,
                                                hitType = HitType.Normal,
                                                isStun = true,
                                                stunDuration = distance,
                                                knockForce = distance / 2.0f});
            }

            if (ownerObj.TryGetComponent<IHitable>(out var owner))
            {
                owner.DecreaseHp(finalDamage);
                stateMachine.ChangeState(stateMachine._groggyState);
            }
        }
        else stateMachine.ChangeState(stateMachine._normalState);
    }

    public override void OnExit(CharacterStateMachine stateMachine)
    {
        Debug.Log("On End Blowed");
        stateMachine.OnCollisionEntered -= _collisionHandler;
    }
}
