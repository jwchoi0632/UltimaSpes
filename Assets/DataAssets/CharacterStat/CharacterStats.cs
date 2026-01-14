using UnityEngine;

[CreateAssetMenu(fileName = "NewStat", menuName = "Stats/CharacterStat")]
public class CharacterStats : ScriptableObject
{
    public float maxHp = 100.0f;
    public float moveSpeed = 5.0f;
    public float jumpForce = 10.0f;
    public float maxJumpTime = 0.5f;
    public float strength = 10.0f;
}
