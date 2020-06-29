using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistOfUnity : Card
{
    public override void Execute()
    {
        base.Execute();
        if(BattleManager.Instance.BattleState != BattleStates.Battle)
            return;
        BattleDelegateHandler.NextEngineEffect += () => BattleManager.Instance.PlayerAttack.PowerTotal += 4;
        BattleDelegateHandler.NextNextEngineEffect += () =>  BattleManager.Instance.PlayerAttack.PowerTotal += 8;
        TrashThis = true;
    }
}
