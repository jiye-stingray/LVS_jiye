using System.Collections;
using UnityEngine;

public class ThornTrapTrigger : MonoBehaviour
{
    private float _attackPower;
    private string _poolKey;
    private bool _triggered;
    private Coroutine _lifetimeCoroutine;

    public void Init(float attackPower, float duration, string poolKey)
    {
        _attackPower = attackPower;
        _poolKey = poolKey;
        _triggered = false;

        if (_lifetimeCoroutine != null)
            StopCoroutine(_lifetimeCoroutine);
        _lifetimeCoroutine = StartCoroutine(LifetimeRoutine(duration));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_triggered) return;
        if (!other.CompareTag("Enemy")) return;

        _triggered = true;
        other.GetComponent<IDamageable>()?.TakeDamage(_attackPower);

        if (_lifetimeCoroutine != null)
            StopCoroutine(_lifetimeCoroutine);
        ReturnToPool();
    }

    private IEnumerator LifetimeRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        _lifetimeCoroutine = null;
        Manager.Instance.Pool.Return(_poolKey, gameObject);
    }
}
