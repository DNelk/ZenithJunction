using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeylineChannel : Card
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
