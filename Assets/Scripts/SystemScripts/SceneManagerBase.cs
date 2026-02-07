using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectPoolManager), typeof(CameraManager))]
public class SceneManagerBase : MonoBehaviour
{
    private static SceneManagerBase _instance;
    public static SceneManagerBase Instance => _instance;
    public ObjectPoolManager _poolManager {  get; private set; }
    public CameraManager _cameraManger { get; private set; }

    [SerializeField] private PlayerCharacter _player;

    public PlayerCharacter Player => _player;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _poolManager = GetComponent<ObjectPoolManager>();
            _cameraManger = GetComponent<CameraManager>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _cameraManger.InitCameraComp(_player);
    }


    void Update()
    {
        
    }
}
