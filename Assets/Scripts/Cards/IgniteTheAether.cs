using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgniteTheAether : Card
{
    public override void Execute()
    {
        foreach (Card c in _myEngine.Stack)
        {
            if (c.CardType == CardTypes.Aether)
            {
                c.AetherTotal *= 3;
                c.TrashThis = true;
            }
                
        }    
    }
}
