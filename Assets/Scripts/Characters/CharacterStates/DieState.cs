using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieState : CharacterStateBase
{
    public DieState(CharacterStateMachine stateMachine) : base(stateMachine) { }

    public override void OnStart()
    {
        base.OnStart();

        Debug.Log("On Die");
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
