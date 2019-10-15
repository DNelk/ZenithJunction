using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyAttack : MonoBehaviour
{
    [SerializeField] protected int _baseDamage;
    protected int _totalDamage; public int TotalDamage => _totalDamage;
    [SerializeField] protected AttackRange _range; public AttackRange Range => _range;
    [SerializeField] protected string _name;
    [SerializeField] protected string _description;
    [SerializeField] protected string _warning;
    public virtual void ExecuteOtherEffect(){}

    public virtual string PrepareAttack()
    {
        _totalDamage = _baseDamage;
        return _warning;
    }

    public virtual bool IsAttackInRange()
    {
        int distance = Mathf.Abs(BattleManager.Instance.Player.Position - BattleManager.Instance.CurrentEnemy.Position);
        switch (Range)
        {
            case AttackRange.Melee:
                if (distance != 0)
                    return false;
                break;
            case AttackRange.Short:
                if (distance == 2)
                    return false;
                break;
            case AttackRange.Long:
                if (distance == 0)
                    return false;
                break;
            default:
                break;
        }
        return true;
    }
}

public enum EnemyAttackType
{
    Attack,
    Move,
    Buff
}
