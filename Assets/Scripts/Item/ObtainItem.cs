using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObtainItem : DropItem
{
    public override void OnInteraction(GameObject causer, InteractionType type, Vector2 force = default)
    {
        if (type != InteractionType.Obtain) return;

        base.OnInteraction(causer, type, force);
    }

    protected override void OnPickup()
    {
        // TODO : 자동 줍기 되는 재화 증가
        Debug.Log("Obtain item");
    }
}