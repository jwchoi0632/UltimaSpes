using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PerceptionLineSensor : PerceptionSensor
{
    public float thickness = 0.5f;
    public bool isVertical = false;
    public bool isUpper = true;

    public override Collider2D[] GetInviewColliders(Transform owner, Vector2 facingDir, LayerMask targetLayer)
    {
        Vector2 dir = facingDir;

        if (isVertical) dir = isUpper ? Vector2.up : Vector2.down;

        RaycastHit2D[] hits = Physics2D.BoxCastAll(owner.position, new Vector2(thickness, thickness), 0, dir, range, targetLayer);

        Collider2D[] results = new Collider2D[hits.Length];
        for (int i = 0; i < hits.Length; i++) results[i] = hits[i].collider;

        return results;
    }

    public override void DrawDebugGizmos(Transform owner, Vector2 facingDir)
    {
        Gizmos.color = Color.red;

        Vector2 dir = facingDir;

        if (isVertical) dir = isUpper ? Vector2.up : Vector2.down;

        Vector3 center = (Vector3)owner.position + (Vector3)dir * (range * 0.5f);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.Euler(0, 0, angle), Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(range, thickness, 0.1f));
        Gizmos.matrix = oldMatrix;
    }
}
