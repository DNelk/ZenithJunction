using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackChange : Card
{
    public override void Execute()
    {
        EngineDelegateHandler.NextEngineEffect += () => BattleManager.Instance.PlayerAttack.AetherTotal += 2;
    }

    public override void ExecuteFailed()
    {
        AetherMod = 0;
    }
}
