using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleEye : Card
{
    public override void Execute()
    {
        base.Execute();
        if (BattleManager.Instance.BattleState == BattleStates.Battle)
        {
            foreach (Engine e in BattleManager.Instance.Engines)
            {
                foreach (Card c in e.Stack)
                {
                    c.Range = AttackRange.Long;
                }
            }
        }

    }
}
