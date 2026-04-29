using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[System.Serializable]
public struct StageScaleData
{
    public int combatScale;
    public int rewardScale;
}

[RequireComponent(typeof(ObjectPoolManager), typeof(CameraManager))]
public class InGameSceneManager : SceneManagerBase, IPoolManageable, ICameraManageable, IPlayerManageable
{
    [SerializeField] private PlayerCharacter _player;
    [SerializeField] private RoomList _roomList;
    [SerializeField] private StageGridList _stageGridList;
    [SerializeField] private Vector2 _roomSize;
    [SerializeField] private StageScaleData _stageScale;
    [SerializeField] private int _stageRewardBudget;

    private Dictionary<RoomCategoryType, List<RoomManager>> _roomListOfType;
    private List<RoomPlan> _roomPlanList;
    private ObjectPoolManager _poolManager;
    private CameraManager _cameraManger;
    private StageScaleController _stageScaleController;
    private UnityEngine.Transform _playerStart;

    private float _randomModifer;
    private float _totalRewardBudget;

    public PlayerCharacter Player => _player;
    public ObjectPoolManager PoolManager => _poolManager;
    public CameraManager CameraManager => _cameraManger;
    public StageScaleController StageScaleController => _stageScaleController;

    protected override void OnAwake()
    {
        base.OnAwake();

        _cameraManger = GetComponent<CameraManager>();
        _poolManager = GetComponent<ObjectPoolManager>();
        _stageScaleController = new StageScaleController(_stageScale);
        _randomModifer = UnityEngine.Random.Range(0.9f, 1.1f);
    }

    protected override void OnStart()
    {
        base.OnStart();

        _roomList.InitDictionary();
        _roomListOfType = new Dictionary<RoomCategoryType, List<RoomManager>>();
        _roomPlanList = new List<RoomPlan>();

        int tryCount = 0;

        while(!ConstructMap(SelectStageGrid()))
        {
            Debug.Log("Map Construct Fail");

            if (tryCount < 100) tryCount++;
            else return;
        }

        InitCurrentGame();

        _cameraManger.InitCameraComp(_player);
    }

    private int SelectStageGrid()
    {
        return UnityEngine.Random.Range(0, _stageGridList.gridList.Count);
    }

    private bool ConstructMap(int gridNum)
    {
        Dictionary<Vector2Int, RoomPlan> roomPlanMap = new Dictionary<Vector2Int, RoomPlan>();

        StageGrid grid = _stageGridList.gridList[gridNum];
        InitRoomPlanMap(ref roomPlanMap, grid);

        List<Vector2Int> path = grid.GetShortestPath();
        if (path.Count <= 0) return false;

        MakeMainPath(roomPlanMap, path);

        List<RoomPlan> zeroGenRooms = path.Select(p => roomPlanMap[p]).ToList();
        ShuffleList(ref zeroGenRooms);

        ConnectRoomPlans(ref zeroGenRooms, roomPlanMap, grid);
        ApplyEventRoomSpecialRules(roomPlanMap);
        DetermineBossRoom(grid, path, roomPlanMap);
        CalculateTotalRewardBudget();
        GenerateMap(roomPlanMap);

        return true;
    }

    private void CalculateTotalRewardBudget()
    {
        int roomCount = _roomPlanList.Count;

        _totalRewardBudget = roomCount * _stageRewardBudget * _randomModifer;

        ShuffleList<RoomPlan>(ref _roomPlanList);

        --roomCount;
        _roomPlanList.RemoveAt(roomCount);

        float normalBudget = _totalRewardBudget * 0.9f;
        float specialBudget = _totalRewardBudget - normalBudget;

        normalBudget /= (float)roomCount;

        foreach (var roomPlan in _roomPlanList)
        {
            roomPlan.rewardBudget = normalBudget;
        }

        _roomPlanList[0].rewardBudget += specialBudget;
    }

    private void GenerateMap(in Dictionary<Vector2Int, RoomPlan> roomPlanMap)
    {
        float w = _roomSize.x;
        float h = _roomSize.y;

        int maxY = roomPlanMap.Keys.Max(p => p.y);

        RoomInitData initData = new RoomInitData();
        initData.randomModi = _randomModifer;

        foreach (var plan in roomPlanMap.Values)
        {
            initData.categoryType = plan.type;
            initData.contextModi = plan.contextMod;
            initData.assignRewardBudget = plan.rewardBudget;

            float correctedY = (maxY - plan.pos.y);
            Vector3 spawnPos = new Vector3(plan.pos.x * w, correctedY * h, 0);

            RoomManager instance = null;

            if (plan.type == RoomCategoryType.None)
            {
                instance = Instantiate(_roomList.noneTypeRoom, spawnPos, Quaternion.identity, this.transform);
            }
            else
            {
                RoomManager prefab = FindSuitablePrefab(plan);

                if (prefab != null)
                {
                    instance = Instantiate(prefab, spawnPos, Quaternion.identity, this.transform);
                }
                else
                {
                    Debug.LogError($"{plan.pos} 위치에 {plan.type} 타입의 적절한 문 구성을 가진 방이 없습니다!");
                }
            }

            initData.combatBudget = _roomList.GetCombatBudget(instance.ContentType);
            initData.rewardBudget = _roomList.GetRewardBudget(instance.ContentType);

            instance.InitRoomManager(initData);

            if (instance.CategoryType == RoomCategoryType.Start) _playerStart = instance.PlayerStart;

            if (!_roomListOfType.ContainsKey(instance.CategoryType))
            {
                _roomListOfType.Add(instance.CategoryType, new List<RoomManager>());
            }

            _roomListOfType[instance.CategoryType].Add(instance);
        }
    }

    private RoomManager FindSuitablePrefab(RoomPlan plan)
    {
        List<RoomManager> candidates = _roomList.GetGroupList(plan.type);

        bool top = plan.connections.ContainsKey(plan.pos + Vector2Int.down);
        bool bottom = plan.connections.ContainsKey(plan.pos + Vector2Int.up);
        bool left = plan.connections.ContainsKey(plan.pos + Vector2Int.left);
        bool right = plan.connections.ContainsKey(plan.pos + Vector2Int.right);

        var result = candidates.Where(room =>
            room.WayInformation.topWay == top &&
            room.WayInformation.bottomWay == bottom &&
            room.WayInformation.leftWay == left &&
            room.WayInformation.rightWay == right
        ).ToList();

        if (result.Count > 0) return result[Random.Range(0, result.Count)];

        Debug.LogError($"[Missing Prefab] Type: {plan.type} | Required Doors -> " +
                       $"Top: {top}, Bottom: {bottom}, Left: {left}, Right: {right} | Pos: {plan.pos}");

        return null;
    }

    private void InitRoomPlanMap(ref Dictionary<Vector2Int, RoomPlan> roomPlanMap, in StageGrid data)
    {
        for (int y = 0; y < data.grid.Count; y++)
        {
            for (int x = 0; x < data.grid[y].columns.Count; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                RoomPlan newRoomPlan = new RoomPlan(pos);

                roomPlanMap.Add(pos, newRoomPlan);

                if (!data.grid[y].columns[x])
                {
                    roomPlanMap[pos].type = RoomCategoryType.None;
                }
                else
                {
                    _roomPlanList.Add(newRoomPlan);
                }
            }
        }
    }

    private void MakeMainPath(in Dictionary<Vector2Int, RoomPlan> roomPlanMap, in List<Vector2Int> shortestPath)
    {
        for (int i = 0; i < shortestPath.Count; i++)
        {
            Vector2Int pos = shortestPath[i];
            RoomPlan plan = roomPlanMap[pos];
            plan.generation = 0;

            if (i == 0) plan.type = RoomCategoryType.Start;
            else if (i == shortestPath.Count - 1) plan.type = RoomCategoryType.End;

            if (i > 0) InternalConnect(pos, shortestPath[i - 1], roomPlanMap);
        }
    }

    private void ConnectRoomPlans(ref List<RoomPlan> zeroGenRooms, in Dictionary<Vector2Int, RoomPlan> roomPlanMap, in StageGrid data)
    {
        Queue<RoomPlan> queue = new Queue<RoomPlan>(zeroGenRooms);

        while (queue.Count > 0)
        {
            RoomPlan current = queue.Dequeue();

            List<Vector2Int> neighborPositions = GetNeighbors(current.pos, roomPlanMap);
            ShuffleList(ref neighborPositions);

            foreach (Vector2Int neighborPos in neighborPositions)
            {
                RoomPlan neighbor = roomPlanMap[neighborPos];

                if (neighbor.generation == -1)
                {
                    neighbor.generation = current.generation + 1;

                    List<Vector2Int> candidates = GetConnectionCandidates(neighborPos, roomPlanMap);

                    ApplyConnectionRule(neighborPos, candidates, roomPlanMap);
                    ApplyPotentiallyEventRule(neighborPos, data, ref neighbor);

                    queue.Enqueue(neighbor);
                }
            }
        }
    }

    private void ApplyConnectionRule(Vector2Int myPos, List<Vector2Int> candidates, in Dictionary<Vector2Int, RoomPlan> roomPlanMap)
    {
        int n = candidates.Count;
        if (n == 0) return;

        int connectionCount = Random.Range(1, n + 1);

        //if (connectionCount == n)
        //{
        //    foreach (var targetPos in candidates)
        //    {
        //        InternalConnect(myPos, targetPos, roomPlanMap);
        //    }
        //}
        //else

        ShuffleList(ref candidates);

        for (int i = 0; i < connectionCount; i++)
        {
            InternalConnect(myPos, candidates[i], roomPlanMap);
        }
    }

    private void ApplyPotentiallyEventRule(Vector2Int neighborPos, in StageGrid data, ref RoomPlan neighbor)
    {
        if (data.IsPotentiallyEvent(neighborPos.x, neighborPos.y))
        {
            neighbor.type = RoomCategoryType.Event;
            // TODO : 이벤트 방 생성 규칙 적용 필요
        }
    }

    private void ApplyEventRoomSpecialRules(Dictionary<Vector2Int, RoomPlan> roomPlanMap)
    {
        foreach (var plan in roomPlanMap.Values)
        {
            if (plan.type == RoomCategoryType.Event)
            {
                DisconnectRooms(plan.pos, plan.pos + Vector2Int.up, roomPlanMap);
                DisconnectRooms(plan.pos, plan.pos + Vector2Int.down, roomPlanMap);

                if (plan.connections.Count == 0)
                {
                    TryForceHorizontalConnect(plan, roomPlanMap);
                }

                _roomPlanList?.Remove(plan);
            }
        }
    }

    private void DisconnectRooms(Vector2Int a, Vector2Int b, in Dictionary<Vector2Int, RoomPlan> roomPlanMap)
    {
        if (roomPlanMap.ContainsKey(a) && roomPlanMap.ContainsKey(b))
        {
            roomPlanMap[a].connections.Remove(b);
            roomPlanMap[b].connections.Remove(a);
        }
    }

    private void TryForceHorizontalConnect(in RoomPlan plan, in Dictionary<Vector2Int, RoomPlan> roomPlanMap)
    {
        Vector2Int[] horizontal = { Vector2Int.left, Vector2Int.right };

        foreach (var dir in horizontal)
        {
            Vector2Int side = plan.pos + dir;

            if (roomPlanMap.ContainsKey(side) && roomPlanMap[side].type != RoomCategoryType.None)
            {
                InternalConnect(plan.pos, side, roomPlanMap);
                return;
            }
        }
    }

    private void DetermineBossRoom(StageGrid data, List<Vector2Int> path, in Dictionary<Vector2Int, RoomPlan> roomPlanMap)
    {
        var potentials = path.Where(p => data.IsPotentiallyBoss(p.x, p.y)).ToList();

        if (potentials.Count > 0)
        {
            roomPlanMap[potentials[Random.Range(0, potentials.Count)]].type = RoomCategoryType.Boss;
        }
    }

    private void InternalConnect(Vector2Int a, Vector2Int b, in Dictionary<Vector2Int, RoomPlan> roomPlanMap)
    {
        if (roomPlanMap.ContainsKey(a) && roomPlanMap.ContainsKey(b))
        {
            roomPlanMap[a].Connect(b);
            roomPlanMap[b].Connect(a);

            if (roomPlanMap[a].type == RoomCategoryType.Start || roomPlanMap[b].type == RoomCategoryType.Start)
            {
                roomPlanMap[a].contextMod = 0.5f;
                roomPlanMap[b].contextMod = 0.5f;
            }
            else if (roomPlanMap[a].type == RoomCategoryType.Event || roomPlanMap[b].type == RoomCategoryType.Event)
            {
                roomPlanMap[a].contextMod = 1.2f;
                roomPlanMap[b].contextMod = 1.2f;
            }
        }
    }

    private List<Vector2Int> GetConnectionCandidates(Vector2Int pos, in Dictionary<Vector2Int, RoomPlan> roomPlanMap)
    {
        List<Vector2Int> candidates = new List<Vector2Int>();
        int myGen = roomPlanMap[pos].generation;

        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var d in dirs)
        {
            Vector2Int next = pos + d;

            if (roomPlanMap.TryGetValue(next, out RoomPlan other))
            {
                if (other.generation != -1 && other.generation <= myGen)
                {
                    candidates.Add(next);
                }
            }
        }
        return candidates;
    }

    private void ShuffleList<T>(ref List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[rnd];
            list[rnd] = temp;
        }
    }

    private List<Vector2Int> GetNeighbors(Vector2Int pos, in Dictionary<Vector2Int, RoomPlan> roomPlanMap)
    {
        List<Vector2Int> res = new List<Vector2Int>();

        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var d in dirs)
        {
            if (roomPlanMap.ContainsKey(pos + d) && roomPlanMap[pos + d].type != RoomCategoryType.None)
            {
                res.Add(pos + d);
            }
        }
            
        return res;
    }

    private void InitCurrentGame()
    {
        _player.transform.position = _playerStart.position;

        InitRoomSpawnPoints();
    }

    private void InitRoomSpawnPoints()
    {
        List<RoomManager> tempList = _roomListOfType[RoomCategoryType.Common];
        ShuffleList<RoomManager>(ref tempList);

        foreach (var room in tempList)
        {
            room.InitSpawnPoints();
        }

        foreach (var room in _roomListOfType[RoomCategoryType.Boss])
        {
            room.InitSpawnPoints();
        }

        tempList = _roomListOfType[RoomCategoryType.Event];
        ShuffleList<RoomManager>(ref tempList);

        foreach (var room in tempList)
        {
            room.InitSpawnPoints();
        }

        foreach (var room in _roomListOfType[RoomCategoryType.End])
        {
            room.InitSpawnPoints();
        }

        foreach (var room in _roomListOfType[RoomCategoryType.Start])
        {
            room.InitSpawnPoints();
        }
    }
}