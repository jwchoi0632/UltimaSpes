using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotTrap : TrapBase
{
    [Header("Shot Trap Option")]
    [SerializeField] protected ProjectileBase _projectilePrefab;
    [SerializeField] protected float _activeTime;
    [SerializeField] protected Transform _firePoint;
    [SerializeField] protected Vector2 _launchDirection;
    [SerializeField] protected float _launchForce = 10.0f;
    [SerializeField] protected LayerMask _targetLayer;
    [SerializeField] protected AttackDataBase _attackData;

    protected override void OnActivate(GameObject Target)
    {
        if (_projectilePrefab == null) return;

        SceneManagerBase sceneManager = SceneManagerBase.Instance;

        if (sceneManager is IPoolManageable poolManageable)
        {
            ProjectileBase projectile = poolManageable.PoolManager.Get<ProjectileBase>(_projectilePrefab);
            //projectile?.Init(gameObject, _hitInfo, _targetLayer);
            _attackData.attackInfo = _hitInfo;
            _attackData.attackInfo.causer = gameObject;
            projectile?.Init(_attackData, _targetLayer);
            projectile?.SetLifeTime(_activeTime);

            AttackContext attackContext = new AttackContext();
            attackContext.spawnPos = _firePoint.position;
            attackContext.direction = _launchDirection;
            attackContext.damageContext.attackMultiplier = 1.0f;
            attackContext.damageContext.baseDamage = _hitInfo.damage;

            projectile?.Launch(attackContext, _launchForce);
        }
    }
}