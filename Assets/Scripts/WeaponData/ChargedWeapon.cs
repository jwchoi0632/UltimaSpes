using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseChargedWeapon", menuName = "Combat/WeaponData/BaseChargedWeapon")]
public class ChargedWeapon : WeaponDataBase, IChargeable, IMoveableOnAttack
{
    [field : SerializeField] public float MinChargeTime { get; private set; }
    [field : SerializeField] public float MaxChargeTime { get; private set; }

    [field : SerializeField] public float MaxSpeedOnGround { get; private set; }
    [field : SerializeField] public float MaxSpeedInAir { get; private set; }
}
