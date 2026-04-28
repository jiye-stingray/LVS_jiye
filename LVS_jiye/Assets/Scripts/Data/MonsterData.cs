using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Data/MonsterData")]
public class MonsterData : ScriptableObject
{
    [SerializeField] private string _monsterName;
    [SerializeField] private Stat   _baseStat = new Stat();
    [SerializeField] private float  _expDrop  = 10f;
    [SerializeField] private string _poolKey;

    public string MonsterName => _monsterName;
    public Stat   BaseStat    => _baseStat;
    public float  ExpDrop     => _expDrop;
    public string PoolKey     => _poolKey;
}
