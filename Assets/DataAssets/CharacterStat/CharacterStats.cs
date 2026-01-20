using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStat", menuName = "Stats/CharacterStat")]
public class CharacterStats : ScriptableObject
{
    // character stat
    [Header("Character Default Stats")]
    public float maxHp = 100.0f;
    public float strength = 10.0f;

    // movement stat
    [Header("Character Movement Stats")]
    public float moveMaxSpeed = 5.0f;
    public float acceleration_sec = 0.3f;
    public float deceleration_sec = 0.2f;

    public float moveMaxSpeed_air = 2.5f;
    public float acceleration_air_sec = 2.0f;
    public float deceleration_air_sec = 2.0f;

    public float moveMaxSpeed_grab = 4.0f;
    public float acceleration_grab_sec = 0.8f;
    public float deceleration_grab_sec = 0.5f;

    public float wallGrabGravityMultiplier = 0.5f;

    //jump stat
    [Header("Character Jump Stats")]
    public float jumpForce = 10.0f;
    public float jumpForce_climbing = 5.0f;
    public float jumpForce_wallGrab = 4.0f;
    public float jumpForce_wallSticking_vertical = 3.0f;
    public float jumpForce_wallSticking_horizontal = 6.0f;
    // 사다리 점프 힘, 매달림 점프 힘 별개 변수 필요
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float dropdownMultiplier = 1.5f;
    public float grapJumpHorizontal = 0.5f;
}
