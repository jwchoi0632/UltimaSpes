using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitType
{
    Normal,
    Blowed,
    Launch,
    FallingHit
}

[CreateAssetMenu(menuName = "Combat/HitPolicy")]
public class HitPolicy : ScriptableObject
{
    public HitType type;
    public HitActionBase action;

    public float iFrame;
    public bool canMove;
    public bool canFlip;
    public bool canHit;
    //public bool canAttack;
    //public string animTrigger;
    public float gravityScale = 1f;
}
