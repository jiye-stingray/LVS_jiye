using System.Collections.Generic;

public class UserInfoData
{
    private readonly Dictionary<string, int> _skillLevels = new();

    public int GetSkillLevel(string skillId)
    {
        return _skillLevels.TryGetValue(skillId, out int level) ? level : 0;
    }

    public bool HasSkill(string skillId)
    {
        return _skillLevels.ContainsKey(skillId);
    }

    public void SetSkillLevel(string skillId, int level)
    {
        _skillLevels[skillId] = level;
    }

    public void LevelUpSkill(string skillId)
    {
        _skillLevels[skillId] = GetSkillLevel(skillId) + 1;
    }

    public void ResetSkillLevels()
    {
        _skillLevels.Clear();
    }
}
