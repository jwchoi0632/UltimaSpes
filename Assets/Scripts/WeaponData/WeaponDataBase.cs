using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChargeable
{
    public float MinChargeTime { get; }
    public float MaxChargeTime { get; }
}

public interface IAimable
{
    public float MinAimTime { get; }
    public float AimMultiplier { get; }
}

public interface IMoveableOnAttack
{
    public float MaxSpeedOnGround { get; }
    public float MaxSpeedInAir { get; }
}

[CreateAssetMenu(fileName = "BaseWeapon", menuName = "Combat/WeaponData/BaseWeapon")]
public class WeaponDataBase : ScriptableObject
{
    [Header("Weapon Data")]
    public float weaponDamage;

    [Header("Attack Data")]
    public AttackDataBase normalAttackData;
    public AttackDataBase chargeAttackData;
}