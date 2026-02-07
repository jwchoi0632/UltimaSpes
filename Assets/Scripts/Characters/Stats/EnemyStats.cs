using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum PatrolType
{
    Range,
    Random
}

[System.Serializable]
public enum AttackSelectType
{
    OnlyFirst,
    Random,
    Decision
}

[System.Serializable]
public class EnemyStats : CharacterStatsBase
{
    [Header("Attack Stats")]
    public AttackSelectType attackSelectType;

    [Header("Patrol Stats")]
    public float patrolRange = 2.0f;
    public PatrolType patrolType = PatrolType.Range;

    [Header("Perception Stats")]
    public float targetLostTime = 1.0f;
    [Range(0.05f, 5.0f)] public float detectionInterval;
    public bool useEnvironmentAwareness = false;
    public bool canJump = false;
    public float minJumpHeight = 1.0f;
    public float minJumpWidth = 1.0f;

    public float wallCheckDist = 0.6f;
    public float groundCheckDist = 0.5f;
    public float ledgeCheckOffset = 0.5f;
    public float ledgeCheckDist = 1.5f;

    public LayerMask targetLayer;
    public LayerMask obstacleLayer;

    [SerializeReference, SubclassSelector] public PerceptionSensor attackSensor;
    [SerializeReference, SubclassSelector] public PerceptionSensor detectionSensor;
}