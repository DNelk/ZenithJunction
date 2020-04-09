﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainOfNails : Card
{
    public override void Execute()
    {
        base.Execute();
        if(BattleManager.Instance.BattleState != BattleStates.Battle)
            return;
        BattleDelegateHandler.NextEngineEffect += () => BattleManager.Instance.PlayerAttack.PowerTotal += 2;
    }
}