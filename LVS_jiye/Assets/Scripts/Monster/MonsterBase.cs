using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBase : UnitBase
{
    private static readonly Color HIT_FLASH_COLOR = Color.red;
    private const float HIT_FLASH_DURATION = 0.1f;

    [SerializeField] private MonsterData _data;

    public MonsterData Data => _data;

    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private Transform _playerTransform;

    private float _speedMultiplier = 1f;
    private Coroutine _slowCoroutine;

    private readonly Dictionary<Collider2D, float> _hitCooldowns = new();

    protected override void Awake()
    {
        if (_data != null)
            _stat = _data.BaseStat;
        base.Awake();
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_rb != null)
            _rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
    }

    protected virtual void OnEnable()
    {
        ResetHp();
        _hitCooldowns.Clear();
        _speedMultiplier = 1f;

        if (_slowCoroutine != null)
        {
            StopCoroutine(_slowCoroutine);
            _slowCoroutine = null;
        }

        if (_spriteRenderer != null)
            _spriteRenderer.color = Color.white;

        if (Manager.Instance?.Player != null)
            _playerTransform = Manager.Instance.Player.transform;
    }

    public void ApplySlow(float speedMultiplier, float duration)
    {
        if (!gameObject.activeInHierarchy) return;

        if (_slowCoroutine != null)
            StopCoroutine(_slowCoroutine);

        _slowCoroutine = StartCoroutine(SlowRoutine(speedMultiplier, duration));
    }

    private IEnumerator SlowRoutine(float speedMultiplier, float duration)
    {
        _speedMultiplier = Mathf.Clamp01(speedMultiplier);
        yield return new WaitForSeconds(duration);
        _speedMultiplier = 1f;
        _slowCoroutine   = null;
    }

    private void FixedUpdate()
    {
        if (_playerTransform == null || _rb == null) return;

        Vector2 dir = ((Vector2)_playerTransform.position - _rb.position).normalized;
        _rb.MovePosition(_rb.position + dir * _data.BaseStat.MoveSpeed * _speedMultiplier * Time.fixedDeltaTime);
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
        if (_spriteRenderer != null && gameObject.activeInHierarchy)
            StartCoroutine(FlashRed());
    }

    private IEnumerator FlashRed()
    {
        _spriteRenderer.color = HIT_FLASH_COLOR;
        yield return new WaitForSeconds(HIT_FLASH_DURATION);
        _spriteRenderer.color = Color.white;
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
