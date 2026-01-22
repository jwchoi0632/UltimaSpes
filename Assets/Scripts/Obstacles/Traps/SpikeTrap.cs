using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : TrapBase
{
    protected override void OnActivate(GameObject Target)
    {
        Target.GetComponent<CharacterBase>().InstantKill();
    }
}
