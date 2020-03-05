using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowcatcherCrash : Card
{
    public override void Execute()
    {
        base.Execute();
        if(BattleManager.Instance.BattleState != BattleStates.Battle)
            return;
        BattleDelegateHandler.AfterDamageEffect += () => BattleManager.Instance.CurrentEnemy.PushPosition(1);
    }
}
