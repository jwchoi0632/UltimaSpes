using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieState : CharacterStateBase
{
    public DieState(CharacterBase character) : base(character) { }

    public override void OnStart()
    {
        _stateName = "»ç¸Á";
        base.OnStart();

        _movement.SetMoveInput(Vector2.zero);
        Debug.Log("On Die");

        if (_owner is IPoolable<EnemyCharacterBase>)
        {
            _owner.SetDeactiveCharacter();
        }
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
