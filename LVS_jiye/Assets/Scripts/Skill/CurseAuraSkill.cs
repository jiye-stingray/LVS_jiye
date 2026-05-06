using UnityEngine;

public class CurseAuraSkill : ActiveSkillBase
{
    private const string SKILL_PREFAB_PATH = "Prefabs/Skill/";

    private CurseAuraSkillData _auraData;
    private GameObject _prefab;
    private bool _isInitialized;

    public override void Init(SkillData data)
    {
        base.Init(data);
        _auraData = data as CurseAuraSkillData;

        _prefab = Resources.Load<GameObject>(SKILL_PREFAB_PATH + _auraData.AuraPrefabName);
        if (_prefab == null)
        {
            Debug.LogError($"[CurseAuraSkill] Prefab not found: {SKILL_PREFAB_PATH}{_auraData.AuraPrefabName}");
            return;
        }

        Manager.Instance.Pool.Register(_prefab);
        _isInitialized = true;
    }

    protected override void OnSkillAction()
    {
        if (!_isInitialized) return;

        Transform target = FindNearestEnemy();
        if (target == null) return;

        var levelData = _auraData.GetAuraLevelData(CurrentLevel);

        GameObject zone = Manager.Instance.Pool.Get(_prefab.name);
        zone.transform.position = target.position;
        zone.GetComponent<CurseAuraTrigger>().Init(
            AttackPower,
            _auraData.TickInterval,
            levelData.radius,
            levelData.duration,
            _prefab.name
        );
    }

    private Transform FindNearestEnemy()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _auraData.SearchRange);
        Transform nearest = null;
        float minDist = float.MaxValue;

        foreach (var col in colliders)
        {
            if (!col.CompareTag("Enemy")) continue;
            float dist = Vector2.Distance(transform.position, col.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = col.transform;
            }
        }

        return nearest;
    }
}
