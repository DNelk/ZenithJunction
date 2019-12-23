using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickySpit : EnemyAttack
{
    public override void ExecuteOtherEffect()
    {
        BattleManager.Instance.Player.ModifyStat(StatType.MovesDOWN, 1, 2, false);
    }
}
