using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindToTracks : Card
{
    public override void Execute()
    {
        base.Execute();
        if(BattleManager.Instance.BattleState != BattleStates.Battle)
            return;
        BattleDelegateHandler.NextEngineEffect += () => BattleManager.Instance.PlayerAttack.AetherTotal += 4;
        BattleManager.Instance.CurrentEnemy.ModifyStat(StatType.MovesDOWN, BattleManager.Instance.EmptyEnginesCount() - 1, 10000);
    }
}
