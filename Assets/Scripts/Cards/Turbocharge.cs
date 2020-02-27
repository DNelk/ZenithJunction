using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turbocharge : Card
{
    public override void Execute()
    {
        base.Execute();
        if(BattleManager.Instance.BattleState != BattleStates.Battle)
            return;
        BuyManager.Instance.BuysRemaining = 0;
    }
}
