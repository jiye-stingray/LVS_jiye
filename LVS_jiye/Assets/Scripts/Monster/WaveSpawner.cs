using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private WaveData[] _waves;
    [SerializeField] private float _minSpawnDistance = 10f;
    [SerializeField] private float _maxSpawnDistance = 15f;

    private readonly List<Coroutine> _spawnCoroutines = new();

    private void Awake()
    {
        Manager.Instance.InitWaveController(this);
    }

    private void Start()
    {
        RegisterPrefabs();
        StartWave();
    }

    private void RegisterPrefabs()
    {
        var registered = new HashSet<string>();

        foreach (var wave in _waves)
        {
            if (wave == null) continue;
            foreach (var entry in wave.Entries)
            {
                if (entry.MonsterData == null) continue;

                string key = entry.MonsterData.PoolKey;
                if (!registered.Add(key)) continue;

                var prefab = Resources.Load<GameObject>($"Prefabs/Monster/{key}");
                if (prefab != null)
                    Manager.Instance.Pool.Register(prefab, 5);
                else
                    Debug.LogError($"[WaveSpawner] Prefab not found: Resources/Prefabs/Monster/{key}");
            }
        }
    }

    public void StartWave()
    {
        StopWave();

        foreach (var wave in _waves)
        {
            if (wave == null) continue;
            foreach (var entry in wave.Entries)
            {
                if (entry.MonsterData == null) continue;
                _spawnCoroutines.Add(StartCoroutine(SpawnRoutine(entry)));
            }
        }
    }

    public void StopWave()
    {
        foreach (var coroutine in _spawnCoroutines)
            if (coroutine != null) StopCoroutine(coroutine);
        _spawnCoroutines.Clear();
    }

    private IEnumerator SpawnRoutine(MonsterSpawnEntry entry)
    {
        while (true)
        {
            yield return new WaitForSeconds(entry.SpawnInterval);
            SpawnMonster(entry.MonsterData);
        }
    }

    private void SpawnMonster(MonsterData data)
    {
        Transform player = Manager.Instance.Player?.transform;
        if (player == null) return;

        Vector2 spawnPos = GetSpawnPosition(player.position);
        GameObject obj = Manager.Instance.Pool.Get(data.PoolKey);
        if (obj != null)
            obj.transform.position = spawnPos;
    }

    private Vector2 GetSpawnPosition(Vector3 playerPos)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Random.Range(_minSpawnDistance, _maxSpawnDistance);
        return new Vector2(
            playerPos.x + Mathf.Cos(angle) * distance,
            playerPos.y + Mathf.Sin(angle) * distance
        );
    }
}
