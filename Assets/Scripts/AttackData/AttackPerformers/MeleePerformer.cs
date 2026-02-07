using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Timeline.TimelinePlaybackControls;

[CreateAssetMenu(fileName = "MeleePerformer", menuName = "Combat/AttackPerformer/MeleePerformer")]
public class MeleePerformer : AttackPerformerBase
{
    public override void Excute(CharacterBase owner, AttackDataBase attackData, AttackContext attackContext)
    {
        base.Excute(owner, attackData, attackContext);

        Debug.Log("Melee Fire");

        if (attackData is IColliderSpawn colliderData)
        {
            float lookDir = owner._movement._isFacingRight ? 1f : -1f;
            Vector2 spawnPos = (Vector2)owner.transform.position + new Vector2(colliderData.Offset.x * lookDir, colliderData.Offset.y);

            Collider2D[] targets = Physics2D.OverlapCircleAll(spawnPos, colliderData.Range, _finalLayer);

            if (owner.TryGetComponent<IAttackable>(out var attacker))
            {
                HitInfo hitInfo = attackData.attackInfo;
                hitInfo.damage = attacker.CalculateDamage(_damageContext);
                hitInfo.causer = owner.gameObject;
                DrawDebugCircle(spawnPos, colliderData.Range, 2.0f);

                foreach (var target in targets)
                {
                    //Vector2 targetDir = (target.transform.position - owner.transform.position).normalized;
                    //float dot = Vector2.Dot(owner.transform.right * lookDir, targetDir);

                    //if (dot > 0.5f) // 대략 전방 120도 이내
                    //{
                    //    hitable.TakeDamage(hitInfo);
                    //}

                    if (target.TryGetComponent<IHitable>(out var hitable))
                    {
                        hitable.TakeDamage(hitInfo);
                    }
                }
            }
        }
    }

    private void DrawDebugCircle(Vector2 center, float radius, float duration)
    {
        int segments = 20;
        float angleStep = 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * angleStep * Mathf.Deg2Rad;
            float angle2 = (i + 1) * angleStep * Mathf.Deg2Rad;

            // Z축을 0으로 명시적으로 고정 (2D 환경)
            Vector3 p1 = new Vector3(center.x + Mathf.Cos(angle1) * radius, center.y + Mathf.Sin(angle1) * radius, 0);
            Vector3 p2 = new Vector3(center.x + Mathf.Cos(angle2) * radius, center.y + Mathf.Sin(angle2) * radius, 0);

            // depthTest를 false로 두면 장애물 뒤에 있어도 보입니다.
            Debug.DrawLine(p1, p2, Color.red, duration, false);
        }
    }
}
