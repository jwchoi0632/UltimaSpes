using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectPoolManager))]
public class SceneManagerBase : MonoBehaviour
{
    private static SceneManagerBase _instance;
    public static SceneManagerBase Instance => _instance;
    public ObjectPoolManager _poolManager {  get; private set; }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _poolManager = GetComponent<ObjectPoolManager>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
