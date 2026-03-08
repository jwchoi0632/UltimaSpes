using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AimableRangedWeapon", menuName = "Combat/WeaponData/AimableRangedWeapon")]
public class AimableRangedWeapon : WeaponDataBase, IAimable, IMoveableOnAttack
{
    [field: SerializeField] public float MinAimTime { get; private set; }
    [field: SerializeField] public float AimMultiplier { get; private set; }
    [field: SerializeField] public float MaxSpeedOnGround { get; private set; }
    [field: SerializeField] public float MaxSpeedInAir { get; private set; }
}
