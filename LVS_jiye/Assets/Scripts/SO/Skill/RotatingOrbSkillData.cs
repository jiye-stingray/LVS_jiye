using UnityEngine;

[System.Serializable]
public struct RotatingOrbLevelData
{
    public int orbCount;
    public float rotateSpeed;
    public float hitInterval;
}

[CreateAssetMenu(fileName = "RotatingOrbSkillData", menuName = "Data/Skill/RotatingOrbSkillData")]
public class RotatingOrbSkillData : SkillData
{
    [SerializeField] private string _orbPrefabName;
    [SerializeField] private RotatingOrbLevelData[] _orbLevelStats;

    public string OrbPrefabName => _orbPrefabName;

    public RotatingOrbLevelData GetOrbLevelData(int level)
    {
        int index = Mathf.Clamp(level - 1, 0, _orbLevelStats.Length - 1);
        return _orbLevelStats[index];
    }

    public new int MaxLevel => _orbLevelStats.Length;
}
