using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeralRage : Card
{
    public override void Execute()
    {
        if(BattleManager.Instance.BattleState != BattleStates.Battle)
            return;
        BuyManager.Instance.BuysRemaining = 0; 
    }
}
