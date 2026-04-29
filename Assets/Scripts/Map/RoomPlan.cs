using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPlan
{
    public Vector2Int pos;
    public RoomCategoryType type = RoomCategoryType.Common;
    public int generation = -1;
    public float contextMod = 1.0f;
    public float rewardBudget = 0.0f;

    public Dictionary<Vector2Int, bool> connections = new Dictionary<Vector2Int, bool>();

    public RoomPlan(Vector2Int pos)
    {
        this.pos = pos;
    }

    public void Connect(Vector2Int neighborPos)
    {
        if (!connections.ContainsKey(neighborPos))
            connections.Add(neighborPos, true);
    }
}