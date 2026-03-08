using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChargeableRangedWeapon", menuName = "Combat/WeaponData/ChargeableRangedWeapon")]
public class ChargeableRangedWeapon : RangedWeapon, IChargeable, IMoveableOnAttack
{
    [field: SerializeField] public float MinChargeTime { get; private set; }
    [field: SerializeField] public float MaxChargeTime { get; private set; }
    [field: SerializeField] public float MaxSpeedOnGround { get; private set; }
    [field: SerializeField] public float MaxSpeedInAir { get; private set; }
}
