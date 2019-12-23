using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveNSpray : EnemyAttack
{
    public override void ExecuteOtherEffect()
    {
        BattleDelegateHandler.EnemyMove += () => BattleManager.Instance.CurrentEnemy.StartCoroutine(BattleManager.Instance.CurrentEnemy.MoveInRange());
        BattleManager.Instance.Player.ModifyStat(StatType.MovesDOWN, 1, 10, false);
    }
}
