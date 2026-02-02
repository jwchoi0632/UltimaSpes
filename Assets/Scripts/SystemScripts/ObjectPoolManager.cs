using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable<T> where T : Component
{
    System.Action<T> OnReturnToPool { get; set; }
}

public class ObjectPoolManager : MonoBehaviour
{
    private Dictionary<int, Stack<Component>> _poolDict = new Dictionary<int, Stack<Component>>();

    public T Get<T>(T prefab) where T : Component
    {
        int keyIndex = prefab.gameObject.GetInstanceID();

        if (!_poolDict.ContainsKey(keyIndex))
        {
            _poolDict[keyIndex] = new Stack<Component>();
        }

        T obj;

        if (_poolDict[keyIndex].Count > 0)
        {
            obj = (T)_poolDict[keyIndex].Pop();
        }
        else
        {
            obj = Instantiate(prefab);
            
            if (obj is IPoolable<T> poolable)
            {
                poolable.OnReturnToPool += (returnObj) => ReturnToPool(keyIndex, returnObj);
            }
        }

        return obj;
    }

    public void ReturnToPool<T>(int keyIndex, T obj) where T : Component
    {
        _poolDict[keyIndex].Push(obj);
    }
}