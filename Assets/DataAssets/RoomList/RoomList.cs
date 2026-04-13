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

[CreateAssetMenu(fileName = "RoomList", menuName = "Map/RoomList")]
public class RoomList : ScriptableObject
{
    public List<RoomGroup> roomGroups;
    public RoomManager noneTypeRoom;

    private Dictionary<RoomCategoryType, List<RoomManager>> _roomDict;

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
    }

    public List<RoomManager> GetGroupList(RoomCategoryType category)
    {
        return _roomDict[category];
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