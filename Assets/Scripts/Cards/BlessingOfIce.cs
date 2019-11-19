using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlessingOfIce : Card
{
    public override void Execute()
    {
        if(BattleManager.Instance.BattleState != BattleStates.Battle)
            return;
        BattleDelegateHandler.EnemyEffect  += () => BattleManager.Instance.CurrentEnemy.AtkDebuff = 1;
        BattleDelegateHandler.NextEngineEffect += () => BattleDelegateHandler.EnemyEffect  += () => BattleManager.Instance.CurrentEnemy.AtkDebuff = 3;
    }
}
