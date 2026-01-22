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
        projectile?.Init(gameObject, _damage, _targetLayer);
        projectile?.Launch(_firePoint.position, _launchDirection, _launchForce);
    }
}