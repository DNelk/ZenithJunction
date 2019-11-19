using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecklessSmash : Card
{
    public override void Execute()
    {
        if(BattleManager.Instance.BattleState != BattleStates.Battle)
            return;
        BattleManager.Instance.Player.TakeDamage(2);
    }
}
