using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TransBallWeapon", menuName = "Combat/WeaponData/TransBallWeapon")]
public class TransBallWeapon : WeaponDataBase, IChargeable, IInstallable, IMoveableOnAttack
{
    [field: SerializeField] public Vector2 InstallSize { get; private set; }
    [field: SerializeField] public float MinChargeTime { get; private set; }
    [field: SerializeField] public float MaxChargeTime { get; private set; }
    [field: SerializeField] public float Distance { get; private set; }
    [field: SerializeField] public float VerticalWeight { get; private set; }
    [field: SerializeField] public LayerMask ObstacleLayer { get; private set; }
    [field: SerializeField] public float MaxSpeedOnGround { get; private set; }
    [field: SerializeField] public float MaxSpeedInAir { get; private set; }
}
