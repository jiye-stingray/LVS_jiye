using System.Collections.Generic;
using UnityEngine;

public class RotatingOrbSkill : ActiveSkillBase
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
        _isInitialized = true;

        OnSkillAction();
    }

    protected override void Update()
    {
        base.Update();

        if (!_isInitialized) return;

        _orbHolder.position = transform.position;

        float speed = _orbData.GetOrbLevelData(CurrentLevel).rotateSpeed;
        _orbHolder.Rotate(0f, 0f, speed * Time.deltaTime);
    }

    protected override void OnLevelUp()
    {
        OnSkillAction();
    }

    protected override void OnSkillAction()
    {
        _orbHolder.position = transform.position;
        ClearOrbs();

        var data = _orbData.GetOrbLevelData(CurrentLevel);
        float angleStep = 360f / data.orbCount;

        for (int i = 0; i < data.orbCount; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            var localPos = new Vector2(
                Mathf.Cos(angle) * ORBIT_RADIUS,
                Mathf.Sin(angle) * ORBIT_RADIUS
            );
            SpawnOrb(localPos);
        }
    }

    private void CreateOrbHolder()
    {
        var holder = new GameObject("OrbHolder");
        _orbHolder = holder.transform;
        _orbHolder.position = transform.position;
    }

    private void SpawnOrb(Vector3 localPos)
    {
        GameObject orb = Manager.Instance.Pool.Get(_orbPrefab.name);
        orb.transform.SetParent(_orbHolder);
        orb.transform.localPosition = localPos;
        orb.GetComponent<OrbHitHandler>().Init(AttackPower, ReturnOrb);
        _orbs.Add(orb);
    }

    private void ReturnOrb(GameObject orb)
    {
        _orbs.Remove(orb);
        orb.transform.SetParent(null);
        Manager.Instance.Pool.Return(_orbPrefab.name, orb);
    }

    private void ClearOrbs()
    {
        for (int i = _orbs.Count - 1; i >= 0; i--)
        {
            var orb = _orbs[i];
            orb.transform.SetParent(null);
            Manager.Instance.Pool.Return(_orbPrefab.name, orb);
        }
        _orbs.Clear();
    }

    private void OnDestroy()
    {
        ClearOrbs();
        if (_orbHolder != null)
            Destroy(_orbHolder.gameObject);
    }
}
