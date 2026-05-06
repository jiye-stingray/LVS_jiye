using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurseAuraTrigger : MonoBehaviour
{
    private float _attackPower;
    private float _tickInterval;
    private string _poolKey;
    private CircleCollider2D _collider;
    private float _defaultRadius;
    private readonly Dictionary<Collider2D, float> _enemyTimers = new();

    private void Awake()
    {
        _collider = GetComponent<CircleCollider2D>();
        _defaultRadius = _collider.radius;
    }

    public void Init(float attackPower, float tickInterval, float radius, float duration, string poolKey)
    {
        _attackPower  = attackPower;
        _tickInterval = tickInterval;
        _poolKey      = poolKey;

        float scale = radius / _defaultRadius;
        transform.localScale = new Vector3(scale, scale, 1f);

        StartCoroutine(LifetimeRoutine(duration));
    }

    private IEnumerator LifetimeRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        ReturnToPool();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        if (!_enemyTimers.ContainsKey(other))
            _enemyTimers[other] = _tickInterval;

        _enemyTimers[other] += Time.fixedDeltaTime;

        if (_enemyTimers[other] >= _tickInterval)
        {
            _enemyTimers[other] = 0f;
            other.GetComponent<IDamageable>()?.TakeDamage(_attackPower);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _enemyTimers.Remove(other);
    }

    private void ReturnToPool()
    {
        _enemyTimers.Clear();
        Manager.Instance.Pool.Return(_poolKey, gameObject);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        _enemyTimers.Clear();
        transform.localScale = Vector3.one;
    }
}
