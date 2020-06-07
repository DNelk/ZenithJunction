using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonBirds : Card
{
    public override void Execute()
    { 
        base.Execute();
        if(BattleManager.Instance.BattleState != BattleStates.Battle)
            return;
       BattleDelegateHandler.EnemyEffect  += () => BattleManager.Instance.CurrentEnemy.AtkDebuff += 2;
    }
}
