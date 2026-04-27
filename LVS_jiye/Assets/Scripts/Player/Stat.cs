using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField] private float _attackPower  = 10f;
    [SerializeField] private float _defense      = 5f;
    [SerializeField] private float _moveSpeed    = 5f;
    [SerializeField] private float _skillCooldown = 1f;
    [SerializeField] private float _expMultiplier = 1f;

    public float AttackPower   => _attackPower;
    public float Defense       => _defense;
    public float MoveSpeed     => _moveSpeed;
    public float SkillCooldown => _skillCooldown;
    public float ExpMultiplier => _expMultiplier;
}
