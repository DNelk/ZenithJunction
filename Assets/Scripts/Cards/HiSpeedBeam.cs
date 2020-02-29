using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiSpeedBeam : Card
{
    public override void Execute()
    {
        base.Execute();
        if (BattleManager.Instance.BattleState == BattleStates.Battle)
            TrashThis = true;
    }
}
