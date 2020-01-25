﻿using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    private ParticleSystem _glow;
    
    private void Start()
    {
        _myCard = GetComponent<Card>();
        _hovering = false;
        _glow = transform.Find("PuffyGlow").GetComponent<ParticleSystem>();
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

            if (_pointerOverTimer >= 0.4f && _myCard.Engine==null && !_myCard.Purchasable)
            {
                if (!_glow.isPlaying)
                    _glow.Play();
            }

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
        if(GameManager.Instance.State == GameState.Battle)
        {
            if (_myCard.Engine != null && _myCard.Engine.EngineState == EngineState.Stacked && BattleManager.Instance.BattleState == BattleStates.ChoosingAction)
            {
                _myCard.Engine.Select();
            }
            else if (_myCard.Purchasable)
            {
                BuyManager.Instance.BuyCard(_myCard);
            }
            else
            {
                CalcOffset();
                transform.SetSiblingIndex(transform.GetSiblingIndex() + 8);
                _myCard.Dragging = true;
            }
        }

        if (GameManager.Instance.State == GameState.Customizing)
        {
            //Check if in deck, if not add to deck and add highlight
        }
        if(_pointerOverTimer >= 0.75f)
            Utils.DestroyCardPreview();
    }

    public override void OnDrag(PointerEventData eventData)
    {
        CalcPosOnMouseMove();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        _myCard.Dragging = false;
        if(BaseScale != Vector3.zero)
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
        if(GameManager.Instance.State == GameState.Battle)
            transform.position = currMousePos + _offset;
    }

    //Snap automatically to an open engine
    public override void OnPointerClick(PointerEventData eventData)
    {
        if(GameManager.Instance.State == GameState.Battle)
        {
            if (_myCard.Engine == null && !_myCard.Purchasable && !_myCard.Dragging && Input.GetMouseButtonUp(1))
            {
                //Find an empty engine
                BattleManager.Instance.GetNextOpenEngine().AddCard(_myCard);
            }
        }

        if (GameManager.Instance.State == GameState.Customizing)
        {
            if(!_myCard.Equipped)
                CustomizeManager.Instance.SelectEquippedCard(_myCard);
            else
                CustomizeManager.Instance.DeselectEquipped(_myCard);
        }
        
    }

    //Generate a card preview
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (BaseScale == Vector3.zero)
        {
            BaseScale = transform.localScale;
            //transform.position += Vector3.up * BaseScale.x * 2;
            if(GameManager.Instance.State == GameState.Battle)
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
        if(_glow.isPlaying)
            _glow.Stop();
    }
    
}
