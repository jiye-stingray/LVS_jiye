using UnityEngine;

public enum SkillType
{
    Passive,
    Active
}

[System.Serializable]
public struct SkillLevelData
{
    public float attackPower;
    public float cooldown;
}

[CreateAssetMenu(fileName = "SkillData", menuName = "Data/SkillData")]
public class SkillData : ScriptableObject
{
    [SerializeField] private string _skillId;
    [SerializeField] private SkillType _skillType;

    [SerializeField] private string _skillName;
    [SerializeField] private string _description;
    [SerializeField] private Sprite _icon;

    // index 0 = 레벨 1 스탯
    [SerializeField] private SkillLevelData[] _levelStats;

    public string SkillId        => _skillId;
    public SkillType SkillType   => _skillType;
    public string SkillName      => _skillName;
    public string Description    => _description;
    public Sprite Icon           => _icon;
    public int MaxLevel          => _levelStats.Length;

    public SkillLevelData GetLevelData(int level)
    {
        int index = Mathf.Clamp(level - 1, 0, _levelStats.Length - 1);
        return _levelStats[index];
    }
}
