using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlessingOfFire : Card
{
    public override void Execute()
    {
        if(BattleManager.Instance.BattleState != BattleStates.Battle)
            return;
        BattleDelegateHandler.NextEngineEffect += () => BattleManager.Instance.PlayerAttack.PowerTotal += 3;
    }
}
