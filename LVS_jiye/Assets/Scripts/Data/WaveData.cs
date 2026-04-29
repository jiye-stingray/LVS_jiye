using System;
using UnityEngine;

[Serializable]
public class MonsterSpawnEntry
{
    [SerializeField] private MonsterData _monsterData;
    [SerializeField] private float _spawnInterval = 2f;

    public MonsterData MonsterData => _monsterData;
    public float SpawnInterval => _spawnInterval;
}

[CreateAssetMenu(menuName = "Data/WaveData")]
public class WaveData : ScriptableObject
{
    [SerializeField] private MonsterSpawnEntry[] _entries;

    public MonsterSpawnEntry[] Entries => _entries;
}
