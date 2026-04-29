using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnPoint : SpawnPointBase
{
    [SerializeField] private List<int> _idList;

    private int selectedId;
    private bool isSelected;

    public override bool InitSpawnPoint(in RoomManager roomManager)
    {
        int totalBudget = roomManager.TotalCombatBudget;

        ShuffleList<int>(ref _idList);
        
        isSelected = false;

        for (int i = 0; i < _idList.Count; ++i)
        {
            int id = _idList[i];
            int cost = GameManager.Instance.MonsterIdData.GetMonsterSpawnCost(id);

            //Debug.Log(roomManager.name + " tcb = " + totalBudget + ", cost : " +  cost);
            if (cost > totalBudget) continue;

            roomManager.DecreaseTotalCombatBudget(cost);
            selectedId = id;
            isSelected = true;

            break;
        }

        return isSelected;
    }

    public override void SpawnObject()
    {
        if (!isSelected) return;

        if (SceneManagerBase.Instance is IPoolManageable poolManager)
        {
            EnemyCharacterBase pref = GameManager.Instance.MonsterIdData.GetMonsterPref(selectedId);
            EnemyCharacterBase instance = poolManager.PoolManager.Get<EnemyCharacterBase>(pref);

            //instance.transform.position = transform.position;
            instance.SetActiveCharacter(transform.position);
        }
    }

    private void ShuffleList<T>(ref List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rnd = UnityEngine.Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[rnd];
            list[rnd] = temp;
        }
    }
}