using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager 
{
    private GameObject _skillHolder = null;
    private readonly Dictionary<int, SkillBase> _skills = new();


    
    public void InitSkillHolder()
    { 
        if(_skillHolder == null)
        {
            _skillHolder = new GameObject("SkillHolder");
            _skillHolder.transform.SetParent(Manager.Instance.Player._skillHolder);
            _skillHolder.transform.localPosition = Vector3.zero;
        }
    
    }

    public void AddSkill(SkillData data)
    {
        if (_skills.ContainsKey(data.SkillId))
        {
            LevelUpSkill(data.SkillId);
            return;
        }

        string className = data.name.EndsWith("Data") ? data.name[..^4] : data.name;
        Type skillType = Type.GetType(className);
        if (skillType == null)
        {
            Debug.LogError($"[SkillManager] Class '{className}' not found.");
            return;
        }

        var skill = _skillHolder.AddComponent(skillType) as SkillBase;
        skill.Init(data);
        _skills[data.SkillId] = skill;
    }

    public void LevelUpSkill(int skillId)
    {
        if (_skills.TryGetValue(skillId, out var skill))
            skill.LevelUp();
        else
            Debug.LogWarning($"[SkillManager] Skill '{skillId}' not found.");
    }

    public bool HasSkill(int skillId) => _skills.ContainsKey(skillId);

    public SkillBase GetSkill(int skillId)
    {
        _skills.TryGetValue(skillId, out var skill);
        return skill;
    }
}
