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

    private void Start()
    {
        u_bodyText.text = "+" + GarbageNum() + " power. Increases based on how many cards you have.";
    }

    private int GarbageNum()
    {
        return (int)Mathf.Pow((((DeckManager.Instance.InDeckCount() + DeckManager.Instance.InDiscardCount()) + 9)/10),2);
    }
}
