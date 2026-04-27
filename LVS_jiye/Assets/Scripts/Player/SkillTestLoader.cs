using UnityEngine;

public class SkillTestLoader : MonoBehaviour
{
    [SerializeField] private RotatingOrbSkillData _orbSkillData;

    private void Start()
    {
        var skill = gameObject.AddComponent<RotatingOrbSkill>();
        skill.Init(_orbSkillData);
    }
}
