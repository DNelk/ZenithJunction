using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Deals 2 + x damage
public class AetherExplosion : Card
{
    public override void Execute()
    {
        base.Execute();
        if(XValue == 0)
            return;
        PowerTotal = XValue + 2;
    }
}
