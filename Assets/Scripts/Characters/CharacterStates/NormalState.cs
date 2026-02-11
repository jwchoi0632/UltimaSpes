using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalState : CharacterStateBase
{
    public NormalState(CharacterBase character) : base(character) 
    { 
        _attackable = true;
        _interactable = true;
    }

    public override void OnStart()
    {
        base.OnStart();
    }
}
