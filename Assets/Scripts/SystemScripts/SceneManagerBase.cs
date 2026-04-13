using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolManageable
{
    public ObjectPoolManager PoolManager { get; }
}

public interface ICameraManageable
{
    public CameraManager CameraManager { get; }
}

public interface IPlayerManageable
{
    public PlayerCharacter Player { get; }
}

public class SceneManagerBase : MonoBehaviour
{
    protected static SceneManagerBase _instance;
    public static SceneManagerBase Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            OnAwake();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        OnStart();
    }

    void Update()
    {
        OnUpdate();
    }

    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }
    protected virtual void OnUpdate() { }
}
