using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectileSpawn 
{ 
    public ProjectileBase ProjectilePref { get; }
    public float ProjectileSpeed { get; }
}

public interface IColliderSpawn
{
    public float Range { get; }
    public Vector2 Offset { get; }
}

[CreateAssetMenu(fileName = "AttackDataBase", menuName = "Combat/AttackData/AttackDataBase")]
[System.Serializable]
public class AttackDataBase : ScriptableObject
{
    public AttackPerformerBase performer;
    public float attackMultiplier;
    public float attackDamage;
    public float recoveryTime;
    public bool breakable;
    public HitInfo attackInfo;
    public LayerMask overrideLayer;
    public LayerMask breakableLayer;
}