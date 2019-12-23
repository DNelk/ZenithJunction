using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeCrust : EnemyAttack
{
    public override void ExecuteOtherEffect()
    {
        base.ExecuteOtherEffect();
        BattleManager.Instance.CurrentEnemy.ModifyStat(StatType.DefenseUP, 2, 2);
    }
}
