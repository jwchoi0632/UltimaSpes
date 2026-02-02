using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RangeAttack", menuName = "Combat/AttackData/RangeAttack")]
[System.Serializable]
public class RangeAttackData : AttackDataBase, IProjectileSpawn
{
    [field: SerializeField] public ProjectileBase ProjectilePref { get; private set; }
    [field: SerializeField] public float ProjectileSpeed { get; private set; }
}