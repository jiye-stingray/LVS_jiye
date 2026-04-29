using System.Collections.Generic;
using UnityEngine;

public class MonsterBase : UnitBase
{
    [SerializeField] private MonsterData _data;

    public MonsterData Data => _data;

    private Rigidbody2D _rb;
    private Transform _playerTransform;

    private readonly Dictionary<Collider2D, float> _hitCooldowns = new();

    protected override void Awake()
    {
        if (_data != null)
            _stat = _data.BaseStat;
        base.Awake();
        _rb = GetComponent<Rigidbody2D>();
        if (_rb != null)
            _rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
    }

    protected virtual void OnEnable()
    {
        ResetHp();
        _hitCooldowns.Clear();

        if (Manager.Instance?.Player != null)
            _playerTransform = Manager.Instance.Player.transform;
    }

    private void FixedUpdate()
    {
        if (_playerTransform == null || _rb == null) return;

        Vector2 dir = ((Vector2)_playerTransform.position - _rb.position).normalized;
        _rb.MovePosition(_rb.position + dir * _data.BaseStat.MoveSpeed * Time.fixedDeltaTime);
    }

    protected override void Die()
    {
        // TODO: 경험치 지급 (_data.ExpDrop)
        string poolKey = _data != null ? _data.PoolKey : gameObject.name;
        Manager.Instance.Pool.Return(poolKey, gameObject);
    }

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
