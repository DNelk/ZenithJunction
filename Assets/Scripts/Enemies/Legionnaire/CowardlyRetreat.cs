using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowardlyRetreat : EnemyAttack
{
    public override void ExecuteOtherEffect()
    {
        base.ExecuteOtherEffect();
        Enemy me = BattleManager.Instance.CurrentEnemy;
        int moves = 1;
        //Stat Check
        moves = StatManager.Instance.StatCheck(moves, me.ActiveStatsList, StatType.MovesDOWN, StatType.NullStat);

        Player player = BattleManager.Instance.Player;
        if (player.Position != 2 && player.Position <= me.Position && me.Position!=2)
            me.ChangePosition(me.Position + moves);
        else if(player.Position != 0 && player.Position >= me.Position && me.Position != 0)
            me.ChangePosition(me.Position - moves);
        
        BattleManager.Instance.enemyPos.UpdatePosition();
    }
    
}
