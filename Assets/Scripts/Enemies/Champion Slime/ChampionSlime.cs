using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionSlime : Enemy
{
    protected override int CalculateDamageWithStatus(int dmg)
    {
        int distance = Mathf.Abs(BattleManager.Instance.Player.Position - BattleManager.Instance.CurrentEnemy.Position);
        if (distance == 0) //The slime's non-newtonian body soaks up close up attacks
        {
            dmg = (int) (dmg * 0.5f);
        }

        return dmg;
    }
}
