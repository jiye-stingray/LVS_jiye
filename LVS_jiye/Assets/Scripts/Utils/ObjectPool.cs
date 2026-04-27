using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private readonly Dictionary<string, Queue<GameObject>> _pools = new();
    private readonly Dictionary<string, GameObject> _prefabs = new();

    public void Register(GameObject prefab, int initialCount = 0)
    {
        string key = prefab.name;
        if (_prefabs.ContainsKey(key)) return;

        _prefabs[key] = prefab;
        _pools[key] = new Queue<GameObject>();

        for (int i = 0; i < initialCount; i++)
        {
            GameObject obj = CreateNew(key);
            obj.SetActive(false);
            _pools[key].Enqueue(obj);
        }
    }

    public GameObject Get(string key)
    {
        if (!_pools.TryGetValue(key, out var pool))
        {
            Debug.LogError($"[ObjectPool] '{key}' is not registered.");
            return null;
        }

        GameObject obj = pool.Count > 0 ? pool.Dequeue() : CreateNew(key);
        obj.SetActive(true);
        return obj;
    }

    public void Return(string key, GameObject obj)
    {
        if (!_pools.ContainsKey(key))
        {
            Object.Destroy(obj);
            return;
        }

        obj.SetActive(false);
        _pools[key].Enqueue(obj);
    }

    private GameObject CreateNew(string key)
    {
        GameObject obj = Object.Instantiate(_prefabs[key]);
        obj.name = key;
        return obj;
    }
}
