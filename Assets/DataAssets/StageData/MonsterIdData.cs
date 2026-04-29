using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

[System.Serializable]
public struct MonsterSpawnData
{
    public int spawnCost;
    public EnemyCharacterBase monsterPref;
}

[System.Serializable]
public struct MonsterSpawnInfo
{
    public int monsterId;
    public MonsterSpawnData spawnData;
}

[CreateAssetMenu(fileName = "MonsterIdData", menuName = "Monster/MonsterIdData")]
public class MonsterIdData : ScriptableObject
{
    public List<MonsterSpawnInfo> monsterSpawnData;

    private Dictionary<int, MonsterSpawnData> _monsterIdDict;

    public void InitMonsterIdDict()
    {
        _monsterIdDict = new Dictionary<int, MonsterSpawnData>();

        foreach (var monsterPref in monsterSpawnData)
        {
            if (!_monsterIdDict.ContainsKey(monsterPref.monsterId))
            {
                _monsterIdDict.Add(monsterPref.monsterId, monsterPref.spawnData);
            }
            else
            {
                Debug.Log("Monster Id : " +  monsterPref.monsterId + " = Å° Áßº¹");
            }
        }
    }

    public EnemyCharacterBase GetMonsterPref(int id)
    {
        if (_monsterIdDict.TryGetValue(id, out var monster))
        {
            return monster.monsterPref;
        }

        return null;
    }

    public int GetMonsterSpawnCost(int id)
    {
        if (_monsterIdDict.TryGetValue(id, out var monster))
        {
            return monster.spawnCost;
        }

        return int.MaxValue;
    }
}