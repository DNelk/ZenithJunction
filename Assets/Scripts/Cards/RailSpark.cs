using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Doubles the power of attack cards

public class RailSpark : Card
{
    public override void Execute()
    {
        base.Execute();
        
        foreach (Card c in _manager.Stack)
        {
            if (c.CardType == CardTypes.Attack)
            {
                c.AtkMod *= 2;
            }
                
        }
    }
}
