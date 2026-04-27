using System.Collections.Generic;
using UnityEngine;

public class RotatingOrbSkill : SkillBase
{
    private const string SKILL_PREFAB_PATH = "Prefabs/Skill/";
    private const float ORBIT_RADIUS = 2.0f;

    private RotatingOrbSkillData _orbData;
    private GameObject _orbPrefab;
    private Transform _orbHolder;
    private readonly List<GameObject> _orbs = new();
    private bool _isInitialized;

    public override void Init(SkillData data)
    {
        base.Init(data);
        _orbData = data as RotatingOrbSkillData;

        _orbPrefab = Resources.Load<GameObject>(SKILL_PREFAB_PATH + _orbData.OrbPrefabName);
        if (_orbPrefab == null)
        {
            Debug.LogError($"[RotatingOrbSkill] Prefab not found at Resources/{SKILL_PREFAB_PATH}{_orbData.OrbPrefabName}");
            return;
        }

        Manager.Instance.Pool.Register(_orbPrefab);
        CreateOrbHolder();
        RefreshOrbs();
        _isInitialized = true;
    }

    private void Update()
    {
        if (!_isInitialized) return;

        _orbHolder.position = transform.position;

        float speed = _orbData.GetOrbLevelData(CurrentLevel).rotateSpeed;
        _orbHolder.Rotate(0f, 0f, speed * Time.deltaTime);
    }

    protected override void OnLevelUp()
    {
        RefreshOrbs();
    }

    private void CreateOrbHolder()
    {
        var holder = new GameObject("OrbHolder");
        holder.transform.position = transform.position;
        _orbHolder = holder.transform;
    }

    private void RefreshOrbs()
    {
        ReturnAllOrbs();

        var data = _orbData.GetOrbLevelData(CurrentLevel);
        float angleStep = 360f / data.orbCount;

        for (int i = 0; i < data.orbCount; i++)
        {
            GameObject orb = Manager.Instance.Pool.Get(_orbPrefab.name);
            orb.transform.SetParent(_orbHolder);

            float angle = angleStep * i * Mathf.Deg2Rad;
            orb.transform.localPosition = new Vector2(
                Mathf.Cos(angle) * ORBIT_RADIUS,
                Mathf.Sin(angle) * ORBIT_RADIUS
            );

            orb.GetComponent<OrbHitHandler>().Init(AttackPower, data.hitInterval);
            _orbs.Add(orb);
        }
    }

    private void ReturnAllOrbs()
    {
        foreach (var orb in _orbs)
        {
            orb.transform.SetParent(null);
            Manager.Instance.Pool.Return(_orbPrefab.name, orb);
        }
        _orbs.Clear();
    }

    private void OnDestroy()
    {
        ReturnAllOrbs();
        if (_orbHolder != null)
            Destroy(_orbHolder.gameObject);
    }
}
