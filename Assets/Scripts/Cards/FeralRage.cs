using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeralRage : Card
{
    public override void Execute()
    {
        base.Execute();
        if(BattleManager.Instance.BattleState != BattleStates.Battle)
            return;
        BuyManager.Instance.BuysRemaining = 0;
        Debug.Log(BattleManager.Instance.EmptyEnginesCount());
        //is first engine
        if (BattleManager.Instance.EmptyEnginesCount() <= 1)
            BattleDelegateHandler.NextEngineEffect += () => BuyManager.Instance.BuysRemaining = 0;
        //Is second engine
        if(BattleManager.Instance.EmptyEnginesCount() == 0)
            BattleDelegateHandler.NextNextEngineEffect += () => BuyManager.Instance.BuysRemaining = 0;

    }
}
