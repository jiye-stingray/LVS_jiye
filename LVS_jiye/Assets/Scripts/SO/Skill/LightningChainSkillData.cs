using UnityEngine;

[System.Serializable]
public struct LightningChainLevelData
{
    public int chainCount;      // 최초 대상 이후 연쇄 횟수
    public float chainRange;    // 연쇄 탐색 반경
    public float chainDelay;    // 연쇄 간 딜레이(초)
}

[CreateAssetMenu(fileName = "LightningChainSkillData", menuName = "Data/Skill/LightningChainSkillData")]
public class LightningChainSkillData : SkillData
{
    [SerializeField] private string _effectPrefabName;
    [SerializeField] private float _initialSearchRange = 15f;
    [SerializeField] private LightningChainLevelData[] _chainLevelStats;

    public string EffectPrefabName    => _effectPrefabName;
    public float InitialSearchRange   => _initialSearchRange;

    public LightningChainLevelData GetChainLevelData(int level)
    {
        int index = Mathf.Clamp(level - 1, 0, _chainLevelStats.Length - 1);
        return _chainLevelStats[index];
    }

    public new int MaxLevel => _chainLevelStats.Length;
}
