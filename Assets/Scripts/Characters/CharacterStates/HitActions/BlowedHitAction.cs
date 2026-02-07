using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlowedHitAction", menuName = "Combat/HitActions/BlowedHit")]
public class BlowedHitAction : HitActionBase
{
    private float _startPos;
    private bool _isCrashed;

    private System.Action<Collision2D> _collisionHandler;

    private IHitable hitableOwner;
    private IGroggyable groggyableOwner;

    public override void OnStart(CharacterStateMachine stateMachine)
    {
        Debug.Log("On Blowed");
        _isCrashed = false;
        _startPos = stateMachine._character.transform.position.x;

        _collisionHandler += (collision) => OnCrashed(stateMachine, collision);
        stateMachine.OnCollisionEntered += _collisionHandler;

        hitableOwner = stateMachine._character as IHitable;
        groggyableOwner = stateMachine._character as IGroggyable;
    }

    public override void OnUpdate(CharacterStateMachine stateMachine, HitInfo info)
    {
        if (_isCrashed) return;

        if (stateMachine._movement._rb.velocity.x < 0.1f &&
            stateMachine._movement._rb.velocity.x > -0.1f)
        {
            stateMachine.OnHitEnd();
        }
    }

    private void OnCrashed(CharacterStateMachine stateMachine, Collision2D collision)
    {
        _isCrashed = true;

        GameObject otherObj = collision.gameObject;
        GameObject ownerObj = stateMachine.gameObject;

        float distance = CalculateDistance(ownerObj.transform.position.x);

        if (distance > 1.0f)
        {
            float finalDamage = CalculateFinalDamage(distance);

            ApplyDamageToOther(ownerObj, otherObj, distance, finalDamage);

            if (hitableOwner != null)
            {
                hitableOwner.DecreaseHp(finalDamage);

                if (groggyableOwner != null)
                {
                    stateMachine.ChangeState(groggyableOwner._groggyState);
                }
            }
        }
        else stateMachine.OnHitEnd();
    }

    public override void OnExit(CharacterStateMachine stateMachine)
    {
        Debug.Log("On End Blowed");
        stateMachine.OnCollisionEntered -= _collisionHandler;
    }

    private float CalculateDistance(float currentPos_x)
    {
        return Mathf.Abs(currentPos_x - _startPos);
    }

    private float CalculateFinalDamage(float distance)
    {
        return distance;
    }

    private void ApplyDamageToOther(GameObject owner, GameObject other, float distance, float damage)
    {
        if (other.TryGetComponent<IHitable>(out var target))
        {
            target.TakeDamage(new HitInfo
            {
                causer = owner,
                damage = damage,
                hitType = HitType.Normal,
                isStun = true,
                stunDuration = distance,
                knockForce = distance / 2.0f
            });
        }
    }
}
