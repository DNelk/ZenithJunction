using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Weightless : Card
{
    public override void Execute()
    {
        base.Execute();
        
        if(BattleManager.Instance.BattleState != BattleStates.Battle)
            return;
        
        foreach (Card c in _myEngine.Stack)
        {
            c.TrashThis = true;
        }

        TrashThis = true;
        foreach (Engine e in BattleManager.Instance.Engines)
        {
            e.OverrideMove = 100000;
        }
    }
}
