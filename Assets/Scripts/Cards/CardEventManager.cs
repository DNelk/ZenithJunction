using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardEventManager : EventTrigger
{
    //Drag Stuff
    private Vector3 _offset;

    private Card _myCard;

    //Card Previews
    private float _pointerOverTimer;
    private bool _hovering;
    public Vector3 BaseScale = Vector3.zero;
    private bool _dontMagnifyUntilHoverAgainHack = false;
    private void Start()
    {
        _myCard = GetComponent<Card>();
        _hovering = false;
    }

    private void Update()
    {
        if (_hovering)
        {
            if (!_dontMagnifyUntilHoverAgainHack)
            {
                if (transform.localScale.x < BaseScale.x * 1.5f)
                    transform.localScale += BaseScale * 5f * Time.deltaTime;
                if (transform.localScale.x > BaseScale.x * 1.5f)
                    transform.localScale = BaseScale * 1.5f;
            }

            _pointerOverTimer += Time.deltaTime;
            if(_pointerOverTimer >= 0.75f)
                Utils.GenerateCardPreview(_myCard);

            if (Input.GetKeyDown(KeyCode.Alpha1) && _myCard.Engine == null && !_myCard.Purchasable)
            {
                BattleManager.Instance.Engines[0].AddCard(_myCard);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && _myCard.Engine == null && !_myCard.Purchasable)
            {
                BattleManager.Instance.Engines[1].AddCard(_myCard);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) && _myCard.Engine == null && !_myCard.Purchasable)
            {
                BattleManager.Instance.Engines[2].AddCard(_myCard);
            }
        }

        if (transform.localScale.x == 0)
        {
            transform.localScale = Vector3.one * 30;
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (_myCard.Engine != null && _myCard.Engine.EngineState == EngineState.Stacked && BattleManager.Instance.BattleState == BattleStates.ChoosingAction)
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
        transform.localScale = BaseScale;
        _dontMagnifyUntilHoverAgainHack = true;
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
        if (BaseScale == Vector3.zero)
        {
            BaseScale = transform.localScale;
            //transform.position += Vector3.up * BaseScale.x * 2;
            transform.SetSiblingIndex(transform.GetSiblingIndex() + 8);
        }
        _hovering = true;
    }
    
    //Make card preview go away
    public override void OnPointerExit(PointerEventData eventData)
    {
        if(_pointerOverTimer >= 0.75f)
            Utils.DestroyCardPreview();
        _pointerOverTimer = 0;
        if(BaseScale != Vector3.zero) 
            transform.localScale = BaseScale;
        //transform.position -= Vector3.up * BaseScale.x * 2;
        BaseScale = Vector3.zero;
        _hovering = false;
        _dontMagnifyUntilHoverAgainHack = false;
    }
    
}
