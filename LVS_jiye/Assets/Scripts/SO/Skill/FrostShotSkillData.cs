using UnityEngine;

[System.Serializable]
public struct FrostShotLevelData
{
    public int pierceCount;       // 추가 관통 수 (0 = 관통 없음)
    public float freezeChance;    // 빙결 확률 (0~1)
    public float slowAmount;      // 이동속도 감소 비율 (0.5 = 50% 감소)
    public float slowDuration;
    public float freezeDuration;
    public float projectileSpeed;
}

[CreateAssetMenu(fileName = "FrostShotSkillData", menuName = "Data/FrostShotSkillData")]
public class FrostShotSkillData : SkillData
{
    [SerializeField] private string _projectilePrefabName;
    [SerializeField] private FrostShotLevelData[] _frostLevelStats;

    public string ProjectilePrefabName => _projectilePrefabName;

    public FrostShotLevelData GetFrostLevelData(int level)
    {
        int index = Mathf.Clamp(level - 1, 0, _frostLevelStats.Length - 1);
        return _frostLevelStats[index];
    }

    public new int MaxLevel => _frostLevelStats.Length;
}
