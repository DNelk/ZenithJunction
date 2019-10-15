using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Deals 2 + x damage
public class AetherExplosion : Card
{
    public override void Execute()
    {
        base.Execute();

        AtkTotal = XValue + 2;
    }
}
