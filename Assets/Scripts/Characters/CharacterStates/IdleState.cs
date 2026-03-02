using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : CharacterStateBase
{
    public IdleState(CharacterBase character) : base(character) { }

    public override void OnStart()
    {
        _stateName = "´ë±â";
        base.OnStart();

        _movement.SetMoveInput(Vector2.zero);
    }
}