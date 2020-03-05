using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockingShinkansenShot : Card
{
    public override void Execute()
    {
        base.Execute();

        if (BattleManager.Instance.EmptyEnginesCount() == 2 && BattleManager.Instance.DamageDealtThisTurn == 0)
        {
            PowerTotal = 20;
            TrashThis = true;
        }
    }
}
