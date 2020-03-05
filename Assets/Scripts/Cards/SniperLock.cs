using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperLock : Card
{
    public override void Execute()
    {
        base.Execute();
        if(BattleManager.Instance.BattleState != BattleStates.Battle)
            return;
        BattleDelegateHandler.NextEngineEffect += () =>
        {
            int distance = Mathf.Abs(BattleManager.Instance.Player.Position - BattleManager.Instance.CurrentEnemy.Position);
            if(distance != 0)
                BattleManager.Instance.PlayerAttack.PowerTotal += 5;
        };
    }
}
