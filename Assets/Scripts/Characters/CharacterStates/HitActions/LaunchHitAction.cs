using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LaunchHitAction", menuName = "Combat/HitActions/LaunchHit")]
public class LaunchHitAction : HitActionBase
{
    public override void OnStart(CharacterStateMachine stateMachine)
    {
        stateMachine._movement.ChangeMoveState(stateMachine._movement._airborneState);
    }

    public override void OnUpdate(CharacterStateMachine stateMachine, HitInfo info)
    {
        if (!stateMachine._movement.CheckGround() &&
            stateMachine._movement._rb.velocity.y > 0.1f) return;

        stateMachine.OnHitEnd();
    }

    public override void OnExit(CharacterStateMachine stateMachine)
    {
        stateMachine._movement.ChangeMoveState(stateMachine._movement._fallingState);
    }
}
