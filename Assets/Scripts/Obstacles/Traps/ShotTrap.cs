using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotTrap : TrapBase
{
    [Header("Shot Trap Option")]
    [SerializeField] protected ProjectileBase _projectilePrefab;
    [SerializeField] protected Transform _firePoint;
    [SerializeField] protected Vector2 _launchDirection;
    [SerializeField] protected float _launchForce = 10.0f;
    [SerializeField] protected LayerMask _targetLayer;

    protected override void OnActivate(GameObject Target)
    {
        if (_projectilePrefab == null) return;

        ProjectileBase projectile = Instantiate(_projectilePrefab, _firePoint.position, _firePoint.rotation);

        projectile.Init(gameObject, _damage, _targetLayer);
        projectile.Launch(_launchDirection, _launchForce);
    }
}
