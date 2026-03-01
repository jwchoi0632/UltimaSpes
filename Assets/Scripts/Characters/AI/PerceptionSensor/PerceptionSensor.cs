using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class PerceptionSensor
{
    public float range;
    public float rangeMultiplier = 2.0f;
    public float defaultRange;

    public void IsChaseModeSensor(bool isChaseMode)
    {
        range = isChaseMode ? defaultRange * rangeMultiplier : defaultRange;
    }

    public abstract Collider2D[] GetInviewColliders(Transform owner, Vector2 facingDir, LayerMask targetLayer);

    public abstract void DrawDebugGizmos(Transform owner, Vector2 facingDir);
}