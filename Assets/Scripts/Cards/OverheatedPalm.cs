using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Doubles the power of attack cards

public class OverheatedPalm : Card
{
    public override void Execute()
    {
        base.Execute();
        if (BattleManager.Instance.BattleState == BattleStates.Battle)
        {
            foreach (Card c in _myEngine.Stack)
            {
                if (c.CardType == CardTypes.Aether)
                {
                    c.TrashThis = true;
                }

            }
            
            foreach (Card c in _myEngine.PoppedCards)
            {
                if (c.CardType == CardTypes.Aether)
                {
                    c.TrashThis = true;
                }

            }
        }
    }
}
