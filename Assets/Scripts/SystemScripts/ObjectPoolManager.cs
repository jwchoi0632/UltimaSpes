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
    private Dictionary<Type, Stack<Component>> _poolDict = new Dictionary<Type, Stack<Component>>();

    public T Get<T>(T prefab) where T : Component
    {
        Type type = typeof(T);

        if (!_poolDict.ContainsKey(type))
        {
            _poolDict[type] = new Stack<Component>();
        }

        T obj;

        if (_poolDict[type].Count > 0)
        {
            obj = (T)_poolDict[type].Pop();
        }
        else
        {
            obj = Instantiate(prefab);
            
            if (obj is IPoolable<T> poolable)
            {
                poolable.OnReturnToPool += ReturnToPool;
            }
        }

        return obj;
    }

    public void ReturnToPool<T>(T obj) where T : Component
    {
        _poolDict[typeof(T)].Push(obj);
    }
}
