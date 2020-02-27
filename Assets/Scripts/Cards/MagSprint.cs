using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagSprint : Card
{
    public override int CalculateAttackTotalWithPosition(int overridePowerTotal = 0)
    {
        return base.CalculateAttackTotalWithPosition(BattleManager.Instance.PlayerAttack.AmtMoved);
    }
}
