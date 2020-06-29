using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SilverProwess : Card
{
    public override void Execute()
    {
        base.Execute();

        bool found = false;
        
        foreach (Card c in _myEngine.Stack)
        {
            if (c.CardName == "Golden Physique")
            {
                found = true;
            }
        }

        foreach (Card c in _myEngine.PoppedCards)
        {
            if (c.CardName == "Golden Physique")
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
