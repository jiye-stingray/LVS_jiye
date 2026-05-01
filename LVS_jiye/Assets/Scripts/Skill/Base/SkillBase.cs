using UnityEngine;
using static Define;

public abstract class SkillBase : MonoBehaviour
{
    protected SkillData _skillData;

    private int _currentLevel = 1;

    public ESkillType SkillType  => _skillData.SkillType;
    public string SkillName      => _skillData.SkillName;
    public string Description    => _skillData.Description;
    public Sprite Icon           => _skillData.Icon;
    public int CurrentLevel      => _currentLevel;
    public bool IsMaxLevel       => _currentLevel >= _skillData.MaxLevel;

    public float AttackPower     => _skillData.GetLevelData(_currentLevel).attackPower;
    public float Cooldown        => _skillData.GetLevelData(_currentLevel).cooldown;

    public virtual void Init(SkillData data)
    {
        _skillData    = data;
        _currentLevel = Mathf.Max(1, Manager.Instance.User.GetSkillLevel(data.SkillId));
    }

    public void LevelUp()
    {
        if (IsMaxLevel) return;
        _currentLevel++;
        Manager.Instance.User.SetSkillLevel(_skillData.SkillId, _currentLevel);
        OnLevelUp();
    }

    protected virtual void OnLevelUp() { }
}
