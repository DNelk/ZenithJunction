using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeOfGarbage : Card
{
    public override void Execute()
    {
        base.Execute();

        PowerTotal = GarbageNum();
    }

    private void Update()
    {
        _cardText = "+" + GarbageNum() + " power. Increases based on how many cards you have.";
        if (_fullSize)
            u_bodyText.text = _cardText;
    }

    private int GarbageNum()
    {
        return (int)Mathf.Pow((((DeckManager.Instance.InDeckCount() + DeckManager.Instance.InDiscardCount()) + 9)/9),2);
    }
}
