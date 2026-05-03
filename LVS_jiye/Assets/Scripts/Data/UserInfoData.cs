using System.Collections.Generic;

public class UserInfoData
{
    private readonly Dictionary<int, int> _skillLevels = new();

    public int GetSkillLevel(int skillId)
    {
        return _skillLevels.TryGetValue(skillId, out int level) ? level : 0;
    }

    public bool HasSkill(int skillId)
    {
        return _skillLevels.ContainsKey(skillId);
    }

    public void SetSkillLevel(int skillId, int level)
    {
        _skillLevels[skillId] = level;
    }

    public void LevelUpSkill(int skillId)
    {
        _skillLevels[skillId] = GetSkillLevel(skillId) + 1;
    }

    public void ResetSkillLevels()
    {
        _skillLevels.Clear();
    }
}
