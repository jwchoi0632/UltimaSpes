using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotTrap : TrapBase
{
    [Header("Shot Trap Option")]
    [SerializeField] ObjectPoolManager poolManager;
    [SerializeField] protected ProjectileBase _projectilePrefab;
    [SerializeField] protected Transform _firePoint;
    [SerializeField] protected Vector2 _launchDirection;
    [SerializeField] protected float _launchForce = 10.0f;
    [SerializeField] protected LayerMask _targetLayer;

    protected override void OnActivate(GameObject Target)
    {
        if (_projectilePrefab == null) return;

        ProjectileBase projectile = poolManager?.Get<ProjectileBase>(_projectilePrefab);
        projectile?.Init(gameObject, _hitInfo, _targetLayer);

        AttackContext attackContext = new AttackContext();
        attackContext.spawnPos = _firePoint.position;
        attackContext.direction = _launchDirection;
        attackContext.damageContext.attackMultiplier = 1.0f;
        attackContext.damageContext.baseDamage = _hitInfo.damage;

        projectile?.Launch(attackContext, _launchForce);
    }
}