using System;
using UnityEngine;

public class OrbHitHandler : MonoBehaviour
{
    private float _attackPower;
    private Action<GameObject> _onHit;

    public void Init(float attackPower, Action<GameObject> onHit)
    {
        _attackPower = attackPower;
        _onHit = onHit;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        if (other.TryGetComponent<IDamageable>(out var damageable))
            damageable.TakeDamage(_attackPower);

        _onHit?.Invoke(gameObject);
        // 재생성은 스킬의 쿨다운이 끝날 때 OnSkillAction에서 담당
    }
}
