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
    public float acceleration_air_sec = 2.0f;
    public float deceleration_air_sec = 2.0f;

    //jump stat
    [Header("Character Jump Stats")]
    public float jumpForce = 10.0f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

}
