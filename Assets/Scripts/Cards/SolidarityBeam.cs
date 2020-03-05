using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidarityBeam : Card
{
    public override void Execute()
    {
        base.Execute();

        if (BattleManager.Instance.EmptyEnginesCount() == 1)
            PowerTotal = 6;
    }
}
