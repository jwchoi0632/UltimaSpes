using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PerceptionLineSensor : PerceptionSensor
{
    public float thickness = 0.5f;
    public float padding = 0.3f;
    public bool isVertical = false;
    public bool isUpper = true;

    public override Collider2D[] GetInviewColliders(Transform owner, Vector2 facingDir, LayerMask targetLayer)
    {
        Vector2 dir = facingDir;

        if (isVertical) dir = isUpper ? Vector2.up : Vector2.down;

        Vector2 origin = (Vector2)owner.position - (dir * padding);
        float totalRange = range + padding;

        Vector2 center = origin + (dir * (totalRange * 0.5f));
        Vector2 size = isVertical ? new Vector2(thickness, totalRange) : new Vector2(totalRange, thickness);

        return Physics2D.OverlapBoxAll(center, size, 0, targetLayer);
    }

    public override void DrawDebugGizmos(Transform owner, Vector2 facingDir)
    {
        Gizmos.color = Color.red;

        Vector2 dir = facingDir;
        if (isVertical) dir = isUpper ? Vector2.up : Vector2.down;

        Vector2 origin = (Vector2)owner.position - (dir * padding);
        float totalRange = range + padding;

        Vector2 center = origin + (dir * (totalRange * 0.5f));
        Vector3 size = isVertical ? new Vector3(thickness, totalRange, 0.1f) : new Vector3(totalRange, thickness, 0.1f);

        Gizmos.DrawWireCube(center, size);
    }
}