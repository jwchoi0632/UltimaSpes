using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectPoolManager), typeof(CameraManager))]
public class InGameSceneManager : SceneManagerBase, IPoolManageable, ICameraManageable, IPlayerManageable
{
    [SerializeField] private PlayerCharacter _player;

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

        _cameraManger.InitCameraComp(_player);
    }
}
