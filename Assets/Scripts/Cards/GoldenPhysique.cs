using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenPhysique : Card
{
    public override void Execute()
    {
        base.Execute();
        
        foreach (Card c in _myEngine.Stack)
        {
            if (c.CardName == "Silver Prowess")
            {
                _myEngine.GoldOrSilverFound = true;
            }
                
        }

        if (_myEngine.GoldOrSilverFound)
        {
            PowerTotal = 10;
            TrashThis = true;
        }
    }
}
