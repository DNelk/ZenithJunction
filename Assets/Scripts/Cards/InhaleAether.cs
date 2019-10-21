using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InhaleAether : Card
{
    public override void Execute()
    {
        BuyManager.Instance.FreeBuysRemaining++;
    }
}
