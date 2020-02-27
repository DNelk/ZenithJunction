using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InhaleAether : Card
{
    public override void Execute()
    {
        base.Execute();
        if(BattleManager.Instance.BattleState != BattleStates.Battle)
            return;
        BuyManager.Instance.FreeBuysRemaining++;
    }
}
