using System.Collections.Generic;
using UnityEngine;

public class MonsterBase : UnitBase
{
    [SerializeField] private MonsterData _data;

    public MonsterData Data => _data;

    private Rigidbody2D _rb;

    protected override void Awake()
    {
        if (_data != null)
            _stat = _data.BaseStat;
        base.Awake();
        _rb = GetComponent<Rigidbody2D>();
        if (_rb != null)
            _rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
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

    private readonly Dictionary<Collider2D, float> _hitCooldowns = new();
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (_hitCooldowns.TryGetValue(other, out float lastHitTime))
            if (Time.time - lastHitTime < 0.2f) return;

        _hitCooldowns[other] = Time.time;

        if (other.TryGetComponent<IDamageable>(out var damageable))
            damageable.TakeDamage(_data.BaseStat.AttackPower);
    }
}
