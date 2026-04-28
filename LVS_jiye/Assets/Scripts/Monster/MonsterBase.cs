using UnityEngine;

public class MonsterBase : UnitBase
{
    [SerializeField] private MonsterData _data;

    public MonsterData Data => _data;

    protected override void Awake()
    {
        if (_data != null)
            _stat = _data.BaseStat;
        base.Awake();
    }

    // 오브젝트 풀에서 꺼낼 때 HP 초기화
    protected virtual void OnEnable()
    {
        ResetHp();
    }

    protected override void Die()
    {
        // TODO: 경험치 지급 (_data.ExpDrop)
        string poolKey = _data != null ? _data.PoolKey : gameObject.name;
        Manager.Instance.Pool.Return(poolKey, gameObject);
    }
}
