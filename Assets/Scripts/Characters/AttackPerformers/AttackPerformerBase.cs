using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackPerformerBase : ScriptableObject
{
    public abstract void Excute(CharacterBase owner, AttackDataBase attackData);
}
