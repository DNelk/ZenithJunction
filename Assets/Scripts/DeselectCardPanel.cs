using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeselectCardPanel : MonoBehaviour
{
    //Collisions
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Card"))
            return;
        Card c = other.gameObject.GetComponent<Card>();
        if(c.Engine == null || (c.Engine != null && c.Engine.EngineState == EngineState.Stacked) || c.Purchasable ||  c.Tweening || c.IsPreview)
            return;
        if (c.Engine != null)
        {
            c.Engine.RemoveCard(c);
            c.Engine = null;
            c.SetEngine(Color.clear, transform.parent);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        OnTriggerEnter2D(other);
    }
}
