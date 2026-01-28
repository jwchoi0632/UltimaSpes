using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HitDatabase", menuName = "Combat/HitDatabase")]
public class HitDatabase : ScriptableObject
{
    public List<HitPolicy> policies;

    public HitPolicy GetPolicy(HitType type) => policies.Find(p => p.type == type);
}