using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GridRow
{
    public List<bool> columns;
}

[System.Serializable]
public struct StageGrid
{
    public List<GridRow> grid;
    public Vector2Int startIndex;
    public Vector2Int endIndex;
    public List<Vector2Int> PotentiallyBossIndexes;
    public List<Vector2Int> PotentiallyEventIndexes;

    public bool IsPotentiallyBoss(int x, int y)
    {
        return PotentiallyBossIndexes.Contains(new Vector2Int(x, y));
    }

    public bool IsPotentiallyEvent(int x, int y)
    {
        return PotentiallyEventIndexes.Contains(new Vector2Int(x, y));
    }

    public List<Vector2Int> GetShortestPath()
    {
        Vector2Int[] directions = {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0)
        };

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        queue.Enqueue(startIndex);
        cameFrom[startIndex] = startIndex;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current == endIndex)
            {
                return ReconstructPath(cameFrom, endIndex);
            }

            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;

                if (IsValidIndex(next.x, next.y))
                {
                    if (grid[next.y].columns[next.x] && !cameFrom.ContainsKey(next))
                    {
                        queue.Enqueue(next);
                        cameFrom[next] = current;
                    }
                }
            }
        }

        return new List<Vector2Int>();
    }

    private bool IsValidIndex(int x, int y)
    {
        return y >= 0 && y < grid.Count && x >= 0 && x < grid[y].columns.Count;
    }

    private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = end;

        while (current != startIndex)
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Add(startIndex);
        path.Reverse();
        return path;
    }
}

[CreateAssetMenu(fileName = "StageGridList", menuName = "Map/StageGridList")]
public class StageGridList : ScriptableObject
{
    public List<StageGrid> gridList;
}