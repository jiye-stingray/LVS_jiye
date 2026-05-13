using UnityEngine;

[System.Serializable]
public struct ThornTrapLevelData
{
    public int trapCount;
    public float duration;
}

[CreateAssetMenu(fileName = "ThornTrapSkillData", menuName = "Data/Skill/ThornTrapSkillData")]
public class ThornTrapSkillData : SkillData
{
    [SerializeField] private string _trapPrefabName;
    [SerializeField] private float _searchRange = 15f;
    [SerializeField] private float _deployRadius = 1.5f;
    [SerializeField] private ThornTrapLevelData[] _trapLevelStats;

    public string TrapPrefabName => _trapPrefabName;
    public float SearchRange => _searchRange;
    public float DeployRadius => _deployRadius;

    public ThornTrapLevelData GetTrapLevelData(int level)
    {
        int index = Mathf.Clamp(level - 1, 0, _trapLevelStats.Length - 1);
        return _trapLevelStats[index];
    }
}
