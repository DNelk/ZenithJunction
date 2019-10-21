using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeralRage : Card
{
    public override void Execute()
    {
        BuyManager.Instance.BuysRemaining = 0; 
    }
}
