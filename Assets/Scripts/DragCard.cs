using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragCard : EventTrigger
{
    //Drag Stuff
    private Vector3 _offset;

    private Card _myCard;

    private void Start()
    {
        _myCard = GetComponent<Card>();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (_myCard.Manager != null && _myCard.Manager.EngineState == EngineState.Stacked)
            return;
        if (_myCard.Purchasable)
        {
            BuyManager.Instance.BuyCard(_myCard);
            return;
        }

        CalcOffset();
        transform.SetSiblingIndex(transform.GetSiblingIndex()+8);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        CalcPosOnMouseMove();
    }

    private void CalcOffset()
    {
        _offset = transform.position -
                  new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
    }

    private void CalcPosOnMouseMove()
    {
        if(_myCard.Manager != null && _myCard.Manager.EngineState == EngineState.Stacked)
            return;
        Vector3 currMousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        transform.position = currMousePos + _offset;
    }
}
