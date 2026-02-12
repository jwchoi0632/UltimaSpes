using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : CharacterStateBase
{
    public IdleState(CharacterBase character) : base(character) { }

    public override void OnStart()
    {
        base.OnStart();

        _movement.SetMoveInput(Vector2.zero);
    }
}