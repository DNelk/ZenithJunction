﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgniteTheAether : Card
{
    public override void Execute()
    {
        base.Execute();
        foreach (Card c in _myEngine.Stack)
        {
            if (c.CardType == CardTypes.Aether)
            {
                c.AetherTotal *= 3;
                if(BattleManager.Instance.BattleState == BattleStates.Battle)
                    c.TrashThis = true;
            }
                
        }    
    }
}
