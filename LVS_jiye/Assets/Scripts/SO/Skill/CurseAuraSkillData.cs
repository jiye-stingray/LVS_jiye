using UnityEngine;

[System.Serializable]
public struct CurseAuraLevelData
{
    public float radius;
    public float duration;
}

[CreateAssetMenu(fileName = "CurseAuraSkillData", menuName = "Data/Skill/CurseAuraSkillData")]
public class CurseAuraSkillData : SkillData
{
    [SerializeField] private string _auraPrefabName;
    [SerializeField] private float _tickInterval = 1f;
    [SerializeField] private float _searchRange = 15f;
    [SerializeField] private CurseAuraLevelData[] _auraLevelStats;

    public string AuraPrefabName => _auraPrefabName;
    public float TickInterval    => _tickInterval;
    public float SearchRange     => _searchRange;

    public CurseAuraLevelData GetAuraLevelData(int level)
    {
        int index = Mathf.Clamp(level - 1, 0, _auraLevelStats.Length - 1);
        return _auraLevelStats[index];
    }

    public new int MaxLevel => _auraLevelStats.Length;
}
