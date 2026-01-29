using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectilePerformer", menuName = "Combat/AttackPerformer/ProjectilePerformer")]
public class ProjectilePerformer : AttackPerformerBase
{
    public override void Excute(CharacterBase owner, AttackDataBase attackData)
    {
        Debug.Log("Projectile Fire");
    }
}
