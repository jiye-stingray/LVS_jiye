using UnityEngine;

public class FrostShotSkill : ActiveSkillBase
{
    private const string SKILL_PREFAB_PATH = "Prefabs/Skill/";

    private FrostShotSkillData _frostData;
    private GameObject _projectilePrefab;
    private bool _isInitialized;

    public override void Init(SkillData data)
    {
        base.Init(data);
        _frostData = data as FrostShotSkillData;

        _projectilePrefab = Resources.Load<GameObject>(SKILL_PREFAB_PATH + _frostData.ProjectilePrefabName);
        if (_projectilePrefab == null)
        {
            Debug.LogError($"[FrostShotSkill] Prefab not found at Resources/{SKILL_PREFAB_PATH}{_frostData.ProjectilePrefabName}");
            return;
        }

        Manager.Instance.Pool.Register(_projectilePrefab);
        _isInitialized = true;
    }

    protected override void OnSkillAction()
    {
        if (!_isInitialized) return;

        Vector2 direction = GetFireDirection();
        var levelData     = _frostData.GetFrostLevelData(CurrentLevel);

        GameObject projectile       = Manager.Instance.Pool.Get(_projectilePrefab.name);
        projectile.transform.position = transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // slowMultiplier: 0 = 완전 정지, 0.5 = 절반 속도, 1 = 정상
        float slowMultiplier = 1f - levelData.slowAmount;

        projectile.GetComponent<FrostShotProjectile>().Init(
            AttackPower,
            levelData.pierceCount,
            slowMultiplier,
            levelData.slowDuration,
            levelData.freezeChance,
            levelData.freezeDuration,
            levelData.projectileSpeed,
            direction,
            _projectilePrefab.name
        );
    }

    private Vector2 GetFireDirection()
    {
        return Manager.Instance.Player.MoveDirection;
    }
}
