using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseMeleeWeapon", menuName = "Combat/WeaponData/BaseMeleeWeapon")]
public class MeleeWeapon : WeaponDataBase, IMoveableOnAttack
{
    [field: SerializeField] public float MaxSpeedOnGround { get; private set; }
    [field: SerializeField] public float MaxSpeedInAir { get; private set; }
}
