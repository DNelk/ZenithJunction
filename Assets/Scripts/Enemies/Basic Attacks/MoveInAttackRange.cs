using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInAttackRange : EnemyAttack
{
    public override void ExecuteOtherEffect()
    {
        BattleDelegateHandler.EnemyMove += () => BattleManager.Instance.CurrentEnemy.StartCoroutine(BattleManager.Instance.CurrentEnemy.MoveInRange());
    }
}
