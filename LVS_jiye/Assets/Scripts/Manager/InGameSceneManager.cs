using System.Collections.Generic;
using UnityEngine;

public class InGameSceneManager : MonoBehaviour
{
    private Dictionary<int, SkillData> _dicSkillData = new();


    private void Awake()
    {
        LoadAllSkillData();
        
    }
    private void Start()
    {

        Manager.Instance.skill.InitSkillHolder();
        Manager.Instance.Wave.Init(Manager.Instance.WaveSpawner);

        Manager.Instance.skill.AddSkill(_dicSkillData[3]);
    }

    private void LoadAllSkillData()
    {
        var allSkillData = Resources.LoadAll<SkillData>("SO/Skill");
        foreach (var data in allSkillData)
            _dicSkillData[data.SkillId] = data;
    }
}
