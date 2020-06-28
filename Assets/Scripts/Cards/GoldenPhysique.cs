using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenPhysique : Card
{
    public override void Execute()
    {
        base.Execute();

        bool found = false;
        
        foreach (Card c in _myEngine.Stack)
        {
            if (c.CardName == "Silver Prowess")
            {
                found = true;
            }
        }

        foreach (Card c in _myEngine.PoppedCards)
        {
            if (c.CardName == "Silver Prowess")
            {
                found = true;
            }
        }
        
        if (found)
        {
            PowerTotal = 10;
            TrashThis = true;
        }
        else
        {
            PowerTotal = 1;
            TrashThis = false;
        }
    }
}
