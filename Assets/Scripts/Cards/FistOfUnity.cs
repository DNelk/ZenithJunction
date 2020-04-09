﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistOfUnity : Card
{
    public override void Execute()
    {
        base.Execute();
        if(BattleManager.Instance.BattleState != BattleStates.Battle)
            return;
        BattleDelegateHandler.NextEngineEffect += () =>
        {
            BattleManager.Instance.PlayerAttack.AetherTotal += 4;
            BattleDelegateHandler.NextEngineEffect += () =>  BattleManager.Instance.PlayerAttack.AetherTotal += 4;
        };
        TrashThis = true;
    }
}