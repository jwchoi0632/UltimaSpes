using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectilePerformer", menuName = "Combat/AttackPerformer/ProjectilePerformer")]
public class ProjectilePerformer : AttackPerformerBase
{
    public override void Excute(CharacterBase owner, AttackDataBase attackData, AttackContext attackContext)
    {
        base.Excute(owner, attackData, attackContext);

        Debug.Log("Projectile Fire");

        if (attackData is IProjectileSpawn projectileSpawn)
        {
            var projectile = SceneManagerBase.Instance._poolManager.Get<ProjectileBase>(projectileSpawn.ProjectilePref);

            attackData.attackInfo.causer = owner.gameObject;
            attackContext.damageContext = _damageContext;

            projectile.Init(attackData, _finalLayer);
            projectile.SetLifeTime(projectileSpawn.LifeTime);
            projectile.Launch(attackContext, projectileSpawn.ProjectileSpeed);
        }
    }
}