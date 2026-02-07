using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PerceptionSectorSensor : PerceptionSensor
{
    [Range(0, 360)] public float viewAngle = 90;

    public override Collider2D[] GetInviewColliders(Transform owner, Vector2 facingDir, LayerMask targetLayer)
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(owner.position, range, targetLayer);

        if (targets.Length == 0) return targets;

        List<Collider2D> filtered = new List<Collider2D>();

        foreach (var target in targets)
        {
            Vector2 dirToTarget = (Vector2)target.transform.position - (Vector2)owner.position;

            float angle = Vector2.Angle(facingDir, dirToTarget);

            if (angle <= viewAngle * 0.5f)
            {
                filtered.Add(target);
            }
        }
        return filtered.ToArray();
    }

    public override void DrawDebugGizmos(Transform owner, Vector2 facingDir)
    {
        Gizmos.color = Color.cyan;

        Vector3 leftDir = Quaternion.Euler(0, 0, -viewAngle * 0.5f) * (Vector3)facingDir;
        Vector3 rightDir = Quaternion.Euler(0, 0, viewAngle * 0.5f) * (Vector3)facingDir;

        Gizmos.DrawLine(owner.position, owner.position + leftDir * range);
        Gizmos.DrawLine(owner.position, owner.position + rightDir * range);
    }
}
