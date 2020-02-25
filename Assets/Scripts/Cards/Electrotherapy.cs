using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Gain x life
public class Electrotherapy : Card
{
    public override void Execute()
    {
        base.Execute();
        if(XValue == 0)
            return;
        if (BattleManager.Instance.BattleState == BattleStates.Battle)
            BattleManager.Instance.Player.GainLife(XValue);
    }
}
