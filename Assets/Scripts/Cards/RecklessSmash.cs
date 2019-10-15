using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecklessSmash : Card
{
    public override void Execute()
    {
        BattleManager.Instance.Player.TakeDamage(2);
    }

    public override void ExecuteFailed()
    {
        AtkTotal = 0;
    }
}
