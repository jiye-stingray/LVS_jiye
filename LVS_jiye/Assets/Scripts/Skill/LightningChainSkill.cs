using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningChainSkill : ActiveSkillBase
{
    private const string SKILL_PREFAB_PATH = "Prefabs/Skill/";

    private LightningChainSkillData _chainData;
    private GameObject _effectPrefab;
    private bool _isInitialized;
    private bool _isChaining;

    public override void Init(SkillData data)
    {
        base.Init(data);
        _chainData = data as LightningChainSkillData;

        _effectPrefab = Resources.Load<GameObject>(SKILL_PREFAB_PATH + _chainData.EffectPrefabName);
        if (_effectPrefab == null)
        {
            Debug.LogError($"[LightningChainSkill] Prefab not found: {SKILL_PREFAB_PATH}{_chainData.EffectPrefabName}");
            return;
        }

        Manager.Instance.Pool.Register(_effectPrefab);
        _isInitialized = true;
    }

    protected override void OnSkillAction()
    {
        if (!_isInitialized || _isChaining) return;

        Transform nearest = FindNearestEnemy(transform.position, _chainData.InitialSearchRange, null);
        if (nearest == null) return;

        var levelData = _chainData.GetChainLevelData(CurrentLevel);
        StartCoroutine(ChainCoroutine(nearest, levelData));
    }

    private IEnumerator ChainCoroutine(Transform firstTarget, LightningChainLevelData levelData)
    {
        _isChaining = true;

        var hit = new HashSet<Transform>();
        Transform current = firstTarget;

        SpawnEffect(current.position);
        current.GetComponent<IDamageable>()?.TakeDamage(AttackPower);
        hit.Add(current);

        int chainsLeft = levelData.chainCount;
        while (chainsLeft > 0)
        {
            if (levelData.chainDelay > 0f)
                yield return new WaitForSeconds(levelData.chainDelay);

            // 대상이 도중에 파괴됐을 경우 현재 위치 기준으로 탐색
            Vector2 searchOrigin = current != null ? (Vector2)current.position : transform.position;

            Transform next = FindNearestEnemy(searchOrigin, levelData.chainRange, hit);
            if (next == null) break;

            SpawnEffect(next.position);
            next.GetComponent<IDamageable>()?.TakeDamage(AttackPower);
            hit.Add(next);

            current = next;
            chainsLeft--;
        }

        _isChaining = false;
    }

    private void SpawnEffect(Vector2 position)
    {
        GameObject effect = Manager.Instance.Pool.Get(_effectPrefab.name);
        effect.transform.position = position;
        effect.GetComponent<LightningChainEffect>()?.Init(_effectPrefab.name);
    }

    private Transform FindNearestEnemy(Vector2 origin, float range, HashSet<Transform> exclude)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(origin, range);
        Transform nearest = null;
        float minDist = float.MaxValue;

        foreach (var col in colliders)
        {
            if (!col.CompareTag("Enemy")) continue;
            if (exclude != null && exclude.Contains(col.transform)) continue;

            float dist = Vector2.Distance(origin, col.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = col.transform;
            }
        }

        return nearest;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        _isChaining = false;
    }
}
