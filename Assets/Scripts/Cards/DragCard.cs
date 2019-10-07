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

    //Card Previews
    private float _pointerOverTimer;
    private bool _hovering;
    
    private void Start()
    {
        _myCard = GetComponent<Card>();
        _hovering = false;
    }

    private void Update()
    {
        if (_hovering && !_myCard.Purchasable)
        {
            _pointerOverTimer += Time.deltaTime;
            if(_pointerOverTimer >= 0.75f)
                Utils.GenerateCardPreview(_myCard);
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (_myCard.Engine != null && _myCard.Engine.EngineState == EngineState.Stacked)
        {
            _myCard.Engine.Select();
        }
        else if (_myCard.Purchasable)
        {
            BuyManager.Instance.BuyCard(_myCard);
            return;
        }
        else
        {
            CalcOffset();
            transform.SetSiblingIndex(transform.GetSiblingIndex() + 8);
            _myCard.Dragging = true;
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        CalcPosOnMouseMove();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        _myCard.Dragging = false;
    }

    private void CalcOffset()
    {
        _offset = transform.position -
                  new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
    }

    //Drag + Offset
    private void CalcPosOnMouseMove()
    {
        if(_myCard.Engine != null && _myCard.Engine.EngineState == EngineState.Stacked)
            return;
        Vector3 currMousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        transform.position = currMousePos + _offset;
    }

    //Snap automatically to an open engine
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (_myCard.Engine == null && !_myCard.Purchasable && !_myCard.Dragging && Input.GetMouseButtonUp(1))
        {
            //Find an empty engine
            BattleManager.Instance.GetNextOpenEngine().AddCard(_myCard);
        }
    }

    //Generate a card preview
    public override void OnPointerEnter(PointerEventData eventData)
    {
        _hovering = true;
    }
    
    //Make card preview go away
    public override void OnPointerExit(PointerEventData eventData)
    {
        if(_pointerOverTimer >= 0.75f)
            Utils.DestroyCardPreview();
        _pointerOverTimer = 0;
        _hovering = false;
    }
}
