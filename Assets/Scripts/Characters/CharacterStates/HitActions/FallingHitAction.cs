using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FallingHitAction", menuName = "Combat/HitActions/FallingHit")]
public class FallingHitAction : HitActionBase
{
    public override void OnStart(CharacterStateMachine stateMachine)
    {
        
    }

    public override void OnUpdate(CharacterStateMachine stateMachine, HitInfo info)
    {
        if (!stateMachine._movement.CheckGround() ||
            stateMachine._movement._rb.velocity.y > 0.1f) return;

        stateMachine.ChangeState(stateMachine._groggyState);
    }

    public override void OnExit(CharacterStateMachine stateMachine)
    {

    }
}
