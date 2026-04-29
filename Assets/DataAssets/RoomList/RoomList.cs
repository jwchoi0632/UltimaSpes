using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct RoomGroup
{
    public RoomCategoryType category;
    public List<RoomManager> prefabs;
}

[System.Serializable]
public struct RoomCombatBudgetData
{
    public RoomContentType content;
    public float combatBudget;
    public float rewardBudget;
}

[CreateAssetMenu(fileName = "RoomList", menuName = "Map/RoomList")]
public class RoomList : ScriptableObject
{
    public List<RoomGroup> roomGroups;
    public RoomManager noneTypeRoom;
    public List<RoomCombatBudgetData> BudgetDatas;

    private Dictionary<RoomCategoryType, List<RoomManager>> _roomDict;
    private Dictionary<RoomContentType, Vector2> _BudgetDict;

    public void InitDictionary()
    {
        _roomDict = new Dictionary<RoomCategoryType, List<RoomManager>>();

        foreach (var group in roomGroups)
        {
            if (!_roomDict.ContainsKey(group.category))
            {
                _roomDict.Add(group.category, group.prefabs);
            }
        }

        _BudgetDict = new Dictionary<RoomContentType, Vector2>();

        foreach (var budget in BudgetDatas)
        {
            if (!_BudgetDict.ContainsKey(budget.content))
            {
                _BudgetDict.Add(budget.content, new Vector2(budget.combatBudget, budget.rewardBudget));
            }
        }
    }

    public float GetCombatBudget(RoomContentType content)
    {
        if (_BudgetDict.TryGetValue(content, out Vector2 Budget))
        {
            return Budget.x;
        }

        return -1;
    }

    public float GetRewardBudget(RoomContentType content)
    {
        if (_BudgetDict.TryGetValue(content, out Vector2 Budget))
        {
            return Budget.y;
        }

        return -1;
    }

    public List<RoomManager> GetGroupList(RoomCategoryType category)
    {
        if (_roomDict.TryGetValue(category, out var list))
        {
            return list;
        }

        return new List<RoomManager>();
    }

    public RoomManager GetRandomRoom(RoomCategoryType type)
    {
        if (_roomDict == null) InitDictionary();

        if (_roomDict.TryGetValue(type, out var list) && list.Count > 0)
        {
            return list[Random.Range(0, list.Count)];
        }

        return null;
    }
}