using System.Collections.Generic;
using UnityEngine;

public class OrbHitHandler : MonoBehaviour
{
    private float _attackPower;
    private float _hitInterval;
    private readonly Dictionary<Collider2D, float> _hitCooldowns = new();

    public void Init(float attackPower, float hitInterval)
    {
        _attackPower = attackPower;
        _hitInterval = hitInterval;
        _hitCooldowns.Clear();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        if (_hitCooldowns.TryGetValue(other, out float lastHitTime))
            if (Time.time - lastHitTime < _hitInterval) return;

        _hitCooldowns[other] = Time.time;

        if (other.TryGetComponent<IDamageable>(out var damageable))
            damageable.TakeDamage(_attackPower);
    }

    private void OnDisable()
    {
        _hitCooldowns.Clear();
    }
}
