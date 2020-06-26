using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisorientingShot : Card
{
    public override void Execute()
    {
        base.Execute();
        if(BattleManager.Instance.BattleState != BattleStates.Battle)
            return;
        Enemy e = BattleManager.Instance.CurrentEnemy;
        
        StatManager.Instance.ClearStats(e.ActiveStatsList);
    }
}
