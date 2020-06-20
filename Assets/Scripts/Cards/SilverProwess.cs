using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SilverProwess : Card
{
    public override void Execute()
    {
        base.Execute();
        
        foreach (Card c in _myEngine.Stack)
        {
            if (c.CardName == "Golden Physique")
            {
                _myEngine.GoldOrSilverFound = true;
            }
                
        }

        if (_myEngine.GoldOrSilverFound)
        {
            PowerTotal = 10;
            TrashThis = true;
            _myEngine.GoldOrSilverFound = false;
        }
        else
        {
            PowerTotal = 1;
            TrashThis = false;
        }
    }
}
