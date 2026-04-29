using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum RoomCategoryType
{
    None,
    Start,
    Common,
    Event,
    Boss,
    End
}

public enum RoomContentType
{
    None,
    Safe,
    Boss,
    Corrider,
    Hazard,
    Puzzle,
    Combat,
    Mixed,
    Event,
    Reward,
    Hidden
}

[Serializable]
public struct RoomWayInformation
{
    public bool leftWay;
    public bool rightWay;
    public bool topWay;
    public bool bottomWay;
}

public struct RoomInitData
{
    public RoomCategoryType categoryType;
    public RoomContentType contentType;
    public float randomModi;
    public float contextModi;
    public float combatBudget;
    public float rewardBudget;
    public float assignRewardBudget;
}

public class RoomManager : MonoBehaviour
{
    [Header("Generate Information")]
    [SerializeField] private RoomWayInformation _roomWayInformation;
    [SerializeField] private RoomCategoryType _roomType;
    [SerializeField] private RoomContentType _roomContentType;

    [Header("Spawn Information")]
    [SerializeField] private Transform _playerStart;
    [SerializeField] private List<MonsterSpawnPoint> _monsterSpawnPoints;

    [Header("Test")]
    [SerializeField] private Text _infoText;

    public RoomWayInformation WayInformation => _roomWayInformation;
    public RoomCategoryType CategoryType => _roomType;
    public RoomContentType ContentType => _roomContentType;

    private float _randomModi;
    private float _combatBudget;
    private float _rewardBudget;
    private float _assignRewardBudget;
    private float _contextModifier;

    private int _totalCombatBudget;
    private int _totalRoomRewardBudget;

    public int TotalCombatBudget => _totalCombatBudget;
    public Transform PlayerStart => _playerStart;

    public void TestFunc()
    {
        InitTestInfo();
        TestSpawn();
    }

    private void InitTestInfo()
    {
        string info = _roomType.ToString() + " ";

        if (_roomWayInformation.topWay) info += "상 ";
        if (_roomWayInformation.bottomWay) info += "하 ";
        if (_roomWayInformation.leftWay) info += "좌 ";
        if (_roomWayInformation.rightWay) info += "우 ";

        _infoText.text = info;
    }

    private void TestSpawn()
    {
        //Debug.Log(gameObject.name + " Start Test Spawn");
        foreach (var spawner in _monsterSpawnPoints)
        {
            spawner.SpawnObject();
        }
    }

    public void InitRoomManager(in RoomInitData initData)
    {
        _roomType = initData.categoryType;
        _roomContentType = initData.contentType;
        _randomModi = initData.randomModi;
        _contextModifier = initData.contextModi;
        _combatBudget = initData.combatBudget;
        _rewardBudget = initData.rewardBudget;
        _assignRewardBudget = initData.assignRewardBudget;
        
        CalculateTotalCombatBudget();
        CalculateTotalRewardBudget();
        //InitSpawnPoints();

        //TestFunc();
    }

    public void DecreaseTotalCombatBudget(int cost) => _totalCombatBudget -= cost;

    private void CalculateTotalCombatBudget()
    {
        float sdm = GameManager.Instance.PlayerStateData.GetStageDepthModifier();
        float plm = GameManager.Instance.PlayerStateData.GetPlayerLevelModifier();

        _totalCombatBudget = (int)(((_combatBudget * sdm * _randomModi) + plm) * _contextModifier);

        //Debug.Log(gameObject.name + " total Combat Budget : " + _totalCombatBudget);
    }

    private void CalculateTotalRewardBudget()
    {
        _totalRoomRewardBudget = Mathf.RoundToInt(_rewardBudget * _assignRewardBudget);
        //Debug.Log(_roomType + " rewardBudget : " + _rewardBudget + ", assignRewardBudget : " + _assignRewardBudget + ", total : " + _totalRoomRewardBudget);
    }

    public void InitSpawnPoints()
    {
        if (_roomType == RoomCategoryType.None) return;

        InitMonsterSpawnPoint();

        TestFunc();
        //TODO : 초기화 시점에 스폰하는 것이 아니라, 방에 최초 진입 시 스폰하도록 수정 필요
    }

    private void InitMonsterSpawnPoint()
    {
        InGameSceneManager stageManager = SceneManagerBase.Instance as InGameSceneManager;

        if (stageManager == null) return;

        StageScaleController ssc = stageManager.StageScaleController;

        ShuffleList<MonsterSpawnPoint>(ref _monsterSpawnPoints);

        foreach (var spawnPoint in _monsterSpawnPoints)
        {
            if (!ssc.IsCombatSpawnable()) return;
            if (_totalCombatBudget < 1) break;

            if (spawnPoint.InitSpawnPoint(this))
            {
                ssc.DecreaseCombatScale();
            }
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

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}