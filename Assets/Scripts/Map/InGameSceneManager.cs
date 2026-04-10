using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectPoolManager), typeof(CameraManager))]
public class InGameSceneManager : SceneManagerBase, IPoolManageable, ICameraManageable, IPlayerManageable
{
    [SerializeField] private PlayerCharacter _player;
    [SerializeField] private RoomList _roomList;
    [SerializeField] private StageGridList _stageGridList;

    private ObjectPoolManager _poolManager;
    private CameraManager _cameraManger;

    public PlayerCharacter Player => _player;
    public ObjectPoolManager PoolManager => _poolManager;
    public CameraManager CameraManager => _cameraManger;

    protected override void OnAwake()
    {
        base.OnAwake();

        _cameraManger = GetComponent<CameraManager>();
        _poolManager = GetComponent<ObjectPoolManager>();
    }

    protected override void OnStart()
    {
        base.OnStart();

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
        StageGrid grid = _stageGridList.gridList[gridNum];

        List<Vector2Int> path = grid.GetShortestPath();

        if (path.Count <= 0) return false;


        // TODO : Create Map

        return true;
    }

    private void InitCurrentGame()
    {
        // TODO : Init Player Transform
    }
}