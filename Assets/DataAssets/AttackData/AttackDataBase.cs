using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackData", menuName = "Combat/AttackData")]
[System.Serializable]
public class AttackDataBase : ScriptableObject
{
    public AttackPerformerBase performer;

    public float minChargeTime;
    public float maxChargeTime;
}