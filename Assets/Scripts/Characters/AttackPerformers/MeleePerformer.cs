using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleePerformer", menuName = "Combat/AttackPerformer/MeleePerformer")]
public class MeleePerformer : AttackPerformerBase
{
    public override void Excute(CharacterBase owner, AttackDataBase attackData)
    {
        Debug.Log("Melee Fire");
    }
}
