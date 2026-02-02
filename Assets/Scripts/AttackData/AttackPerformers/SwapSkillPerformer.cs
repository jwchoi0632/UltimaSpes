using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SwapSkillPerformer", menuName = "Combat/AttackPerformer/SwapSkillPerformer")]
public class SwapSkillPerformer : AttackPerformerBase
{
    public override void Excute(CharacterBase owner, AttackDataBase attackData, AttackContext attackContext)
    {
        Debug.Log("Swap Skill Fire");
    }
}
