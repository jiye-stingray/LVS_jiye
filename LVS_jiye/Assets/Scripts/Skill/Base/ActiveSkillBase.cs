using UnityEngine;

public abstract class ActiveSkillBase : SkillBase
{
    private float _timer;

    public bool IsReady           => _timer >= Cooldown;
    public float CooldownProgress => Mathf.Clamp01(_timer / Cooldown);

    protected virtual void Update()
    {
        if (IsReady) return;

        _timer += Time.deltaTime;
        if (IsReady)
        {
            _timer = Cooldown;
            OnSkillAction();
        }
    }

    protected abstract void OnSkillAction();
}
