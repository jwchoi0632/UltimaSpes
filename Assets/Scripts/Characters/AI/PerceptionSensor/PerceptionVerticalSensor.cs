using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.UI.Image;

[System.Serializable]
public class PerceptionVerticalSensor : PerceptionSensor
{
    public float footDefaultRange = 0.4f;
    private float footRange = 0.4f;
    private bool _isDescending;

    public PerceptionVerticalSensor() : base() { footRange = footDefaultRange; }

    public override void ApplyRangeMultiplier(bool isMultipled)
    {
        base.ApplyRangeMultiplier(isMultipled);

        _isDescending = isMultipled;
        footRange = isMultipled ? footDefaultRange * rangeMultiplier : footDefaultRange;
    }

    public override Collider2D[] GetInviewColliders(Transform owner, Vector2 facingDir, LayerMask targetLayer)
    {
        Vector2 start = owner.position;
        Vector2 end = owner.position;

        if (_isDescending) // 아래 이동
        {
            start += Vector2.down * 0.5f;
            end += Vector2.down * footRange;
        }
        else // 위 이동
        {
            start += Vector2.up * range;
            end += Vector2.down * footRange;
        }

        return Physics2D.OverlapAreaAll(start, end, targetLayer);
    }

    public override void DrawDebugGizmos(Transform owner, Vector2 facingDir)
    {
        Gizmos.color = Color.green;
        Vector3 start = owner.position + Vector3.up * range;
        Vector3 end = owner.position + Vector3.down * footRange;
        Gizmos.DrawLine(start, end);

        Gizmos.DrawSphere(start, 0.05f);
        Gizmos.DrawSphere(end, 0.05f);
    }
}