﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Doubles the power of attack cards

public class RailCharge : Card
{
    public override void Execute()
    {
        base.Execute();
        
        foreach (Card c in _myEngine.Stack)
        {
            if (c.CardType == CardTypes.Attack)
            {
                c.AtkTotal *= 2;
            }
                
        }
    }
}