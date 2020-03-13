using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InfiniteAmmo : Card
{
    public override void Execute()
    {
        base.Execute();
        
        if(BattleManager.Instance.BattleState != BattleStates.Battle)
            return;

        TrashThis = true;
        
        foreach (Engine e in BattleManager.Instance.Engines)
        {
            foreach (Card c in e.Stack)
            {
                if (c.CardType == CardTypes.Aether)
                {
                    c.PowerValue = 3;
                    c.Range = AttackRange.Short;
                    c.CardType = CardTypes.Attack;
                    c.AssignUI();
                    c.Engine.UpdateUICounts();
                }
            }
        }
    }
}
