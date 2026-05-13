using System.Collections.Generic;
using UnityEngine;

public class ThornTrapSkill : ActiveSkillBase
{
    private const string SKILL_PREFAB_PATH = "Prefabs/Skill/";
    private const float SAMPLE_INTERVAL = 0.3f;
    private const int MAX_HISTORY = 20;
    private const float MIN_SAMPLE_DISTANCE = 0.5f;

    private ThornTrapSkillData _trapData;
    private GameObject _prefab;
    private readonly List<Vector2> _positionHistory = new();
    private float _sampleTimer;

    public override void Init(SkillData data)
    {
        base.Init(data);
        _trapData = data as ThornTrapSkillData;
        _prefab = Resources.Load<GameObject>(SKILL_PREFAB_PATH + _trapData.TrapPrefabName);
        Manager.Instance.Pool.Register(_prefab);
    }

    protected override void Update()
    {
        base.Update();

        _sampleTimer += Time.deltaTime;
        if (_sampleTimer >= SAMPLE_INTERVAL)
        {
            _sampleTimer = 0f;
            RecordPlayerPosition();
        }
    }

    protected override void OnSkillAction()
    {
        if (_positionHistory.Count == 0) return;

        var levelData = _trapData.GetTrapLevelData(CurrentLevel);
        int count = Mathf.Min(levelData.trapCount, _positionHistory.Count);

        for (int i = 0; i < count; i++)
        {
            // 히스토리 전체 구간에서 균등 간격으로 인덱스 선택
            int index = count == 1
                ? _positionHistory.Count - 1
                : (_positionHistory.Count - 1) * i / (count - 1);

            GameObject trap = Manager.Instance.Pool.Get(_prefab.name);
            trap.transform.position = _positionHistory[index];
            trap.GetComponent<ThornTrapTrigger>().Init(AttackPower, levelData.duration, _prefab.name);
        }
    }

    private void RecordPlayerPosition()
    {
        Vector2 pos = Manager.Instance.Player.transform.position;

        if (_positionHistory.Count > 0 &&
            Vector2.Distance(_positionHistory[_positionHistory.Count - 1], pos) < MIN_SAMPLE_DISTANCE)
            return;

        _positionHistory.Add(pos);
        if (_positionHistory.Count > MAX_HISTORY)
            _positionHistory.RemoveAt(0);
    }
}
