using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InstallWeapon", menuName = "Combat/WeaponData/InstallWeapon")]
public class InstallWeapon : WeaponDataBase, IInstallable
{
    [field: SerializeField] public Vector2 InstallSize { get; private set; }
    [field: SerializeField] public float Distance { get; private set; }
    [field: SerializeField] public float VerticalWeight { get; private set; }
    [field: SerializeField] public LayerMask ObstacleLayer { get; private set; }
}
