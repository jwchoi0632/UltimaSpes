using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalState : CharacterStateBase, IAttackable, IMoveableState, IJumpableState
{
    public NormalState(CharacterStateMachine stateMachine) : base(stateMachine) { }

    public void Attack()
    {

    }
}
