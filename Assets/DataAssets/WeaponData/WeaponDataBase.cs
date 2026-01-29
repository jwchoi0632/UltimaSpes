using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Combat/WeaponData")]
public class WeaponDataBase : ScriptableObject
{
    public float weaponDamage;

    public bool isChargeable;
    public bool isAimable;

    public float aimMultiPlier = 5.0f;

    public AttackDataBase normalAttackData;
    public AttackDataBase chargeAttackData;
}