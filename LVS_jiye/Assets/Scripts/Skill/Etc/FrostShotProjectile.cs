using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostShotProjectile : MonoBehaviour
{
    private const float MAX_LIFETIME = 5f;

    private float _attackPower;
    private int _pierceLeft;
    private float _slowMultiplier;
    private float _slowDuration;
    private float _freezeChance;
    private float _freezeDuration;
    private float _speed;
    private Vector2 _direction;
    private string _poolKey;
    private bool _returning;

    private readonly HashSet<Collider2D> _hitTargets = new();

    public void Init(float attackPower, int pierceCount, float slowMultiplier, float slowDuration,
                     float freezeChance, float freezeDuration, float speed, Vector2 direction, string poolKey)
    {
        _attackPower    = attackPower;
        _pierceLeft     = pierceCount;
        _slowMultiplier = slowMultiplier;
        _slowDuration   = slowDuration;
        _freezeChance   = freezeChance;
        _freezeDuration = freezeDuration;
        _speed          = speed;
        _direction      = direction.normalized;
        _poolKey        = poolKey;
        _returning      = false;

        StartCoroutine(LifetimeRoutine());
    }

    private void OnDisable()
    {
        _hitTargets.Clear();
        _returning = false;
        StopAllCoroutines();
    }

    private void FixedUpdate()
    {
        if (_returning) return;
        transform.Translate(_direction * _speed * Time.fixedDeltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_returning) return;
        if (!other.CompareTag("Enemy")) return;
        if (_hitTargets.Contains(other)) return;

        _hitTargets.Add(other);

        if (other.TryGetComponent<IDamageable>(out var damageable))
            damageable.TakeDamage(_attackPower);

        if (other.TryGetComponent<MonsterBase>(out var monster))
        {
            bool isFrozen = Random.value < _freezeChance;
            if (isFrozen)
                monster.ApplySlow(0f, _freezeDuration);
            else
                monster.ApplySlow(_slowMultiplier, _slowDuration);
        }

        if (_pierceLeft <= 0)
            ReturnToPool();
        else
            _pierceLeft--;
    }

    private IEnumerator LifetimeRoutine()
    {
        yield return new WaitForSeconds(MAX_LIFETIME);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (_returning) return;
        _returning = true;
        Manager.Instance.Pool.Return(_poolKey, gameObject);
    }
}
