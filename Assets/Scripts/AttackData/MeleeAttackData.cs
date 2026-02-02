using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeAttack", menuName = "Combat/AttackData/MeleeAttack")]
[System.Serializable]
public class MeleeAttackData : AttackDataBase, IColliderSpawn
{
    [field : SerializeField] public float Range { get; private set; }
    [field : SerializeField] public Vector2 Offset { get; private set; }
}
