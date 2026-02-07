using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultHitAction", menuName = "Combat/HitActions/DefaultHit")]
public class DefaultHitAction : HitActionBase
{
    public override void OnStart(CharacterStateMachine stateMachine)
    {
        
    }

    public override void OnUpdate(CharacterStateMachine stateMachine, HitInfo info)
    {
        if (!stateMachine._movement.CheckGround() ||
            stateMachine._movement._rb.velocity.y > 0.1f) return;
        
        if (stateMachine._movement.CheckGround() &&
            stateMachine._movement._rb.velocity.x > - 0.1f &&
            stateMachine._movement._rb.velocity.x < 0.1f)
        {
            stateMachine.OnHitEnd();
        }
    }

    public override void OnExit(CharacterStateMachine stateMachine)
    {
        
    }
}
