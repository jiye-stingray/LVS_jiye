using UnityEngine;

public abstract class UnitBase : MonoBehaviour, IDamageable
{
    [SerializeField] protected Stat _stat = new Stat();
    public Stat Stat => _stat;

    public float CurrentHp { get; private set; }
    public float MaxHp => _stat.MaxHp;

    protected virtual void Awake()
    {
        CurrentHp = MaxHp;
    }

    protected void ResetHp()
    {
        CurrentHp = MaxHp;
    }

    public virtual void TakeDamage(float amount)
    {
        float damage = Mathf.Max(0f, amount - _stat.Defense);
        CurrentHp = Mathf.Max(0f, CurrentHp - damage);

        if (CurrentHp <= 0f)
            Die();
    }

    protected abstract void Die();
}
