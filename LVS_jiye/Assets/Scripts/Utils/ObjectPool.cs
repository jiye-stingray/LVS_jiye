using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    private readonly Dictionary<string, Queue<GameObject>> _pools = new();
    private readonly Dictionary<string, GameObject> _prefabs = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

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
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        obj.transform.SetParent(transform);
        _pools[key].Enqueue(obj);
    }

    private GameObject CreateNew(string key)
    {
        GameObject obj = Instantiate(_prefabs[key], transform);
        obj.name = key;
        return obj;
    }
}
