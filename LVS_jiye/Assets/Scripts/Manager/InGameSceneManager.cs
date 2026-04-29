using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class InGameSceneManager : MonoBehaviour
{
    [SerializeField] private RotatingOrbSkillData _orbSkillData;

    private void Start()
    {
        // dummy & test 
        Manager.Instance.skill.InitSkillHolder();
        Manager.Instance.skill.AddSkill(_orbSkillData);

        Manager.Instance.Wave.Init(Manager.Instance.WaveSpawner);


    }
}
