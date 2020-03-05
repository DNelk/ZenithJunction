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
        if (e.ActiveStats.ContainsKey(StatType.AttackUP))
        {
            e.ActiveStats[StatType.AttackUP].Value = 0;
            e.ActiveStats[StatType.AttackUP].TurnsLeft = 0;

        }
        if (e.ActiveStats.ContainsKey(StatType.DefenseUP))
        {
            e.ActiveStats[StatType.DefenseUP].Value = 0;
            e.ActiveStats[StatType.DefenseUP].TurnsLeft = 0;

        }
        if (e.ActiveStats.ContainsKey(StatType.MovesUP))
        {
            e.ActiveStats[StatType.MovesUP].Value = 0;
            e.ActiveStats[StatType.MovesUP].TurnsLeft = 0;

        }
    }
}
