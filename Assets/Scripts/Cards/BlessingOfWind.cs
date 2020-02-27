using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlessingOfWind : Card
{
    public override void Execute()
    {
        base.Execute();
        if(BattleManager.Instance.BattleState != BattleStates.Battle)
            return;
        BattleDelegateHandler.NextEngineEffect += () => BattleManager.Instance.PlayerAttack.MoveTotal += 2;
    }
}
