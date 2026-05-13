using UnityEngine;
using static Define;

[System.Serializable]
public struct SkillLevelData
{
    public float attackPower;
    public float cooldown;
}

[CreateAssetMenu(fileName = "SkillData", menuName = "Data/Skill/SkillData")]
public class SkillData : ScriptableObject
{
    [SerializeField] private int _skillId;
    [SerializeField] private ESkillType _skillType;

    [SerializeField] private string _skillName;
    [TextArea(3, 6)]
    [SerializeField] private string _description;
    [SerializeField] private Sprite _icon;

    // index 0 = 레벨 1 스탯
    [SerializeField] private SkillLevelData[] _levelStats;

    public int SkillId          => _skillId;
    public ESkillType SkillType      => _skillType;
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
