using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Deals 2 + x damage
public class SteamExplosion : Card
{
    public override void Execute()
    {
        base.Execute();

        AtkMod = XValue + 2;
    }
}
