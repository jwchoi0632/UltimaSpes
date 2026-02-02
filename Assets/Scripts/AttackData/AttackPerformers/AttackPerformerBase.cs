using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AttackContext
{
    public Vector2 spawnPos;
    public Vector2 direction;
    public float chargeRatio;
    public DamageContext damageContext;
}

public abstract class AttackPerformerBase : ScriptableObject
{
    protected LayerMask _finalLayer;
    protected DamageContext _damageContext;

    public virtual void Excute(CharacterBase owner, AttackDataBase attackData, AttackContext attackContext)
    {
        _finalLayer = (attackData.overrideLayer != 0) ? attackData.overrideLayer : owner.TargetLayer;
        _damageContext.attackMultiplier = attackData.attackMultiplier;
        _damageContext.attackDamage = attackData.attackDamage;
    }
}
