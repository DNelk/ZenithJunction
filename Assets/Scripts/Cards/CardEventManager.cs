using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Collections;
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
    public bool _hovering;
    public Vector3 BaseScale = Vector3.zero;
    private bool _dontMagnifyUntilHoverAgainHack = false;

    private ParticleSystem _glow;
    private Vector3 _glowScale = Vector3.zero;
    private Gradient _inEngineColor, _baseColor;

    private void Start()
    {
        _myCard = GetComponent<Card>();
        _hovering = false;
        _glow = transform.Find("PuffyGlow").GetComponent<ParticleSystem>();
        _glowScale = _glow.transform.localScale;

        Color whiteSmoke = new Color(0.5f,0.8f,1);
        _baseColor = new Gradient();
        _baseColor.SetKeys(new GradientColorKey[]{new GradientColorKey(whiteSmoke, 0.0f)}, 
            new GradientAlphaKey[]{new GradientAlphaKey(0.0f, 0.0f), new GradientAlphaKey(1.0f, 0.524f), new GradientAlphaKey(0.0f, 1.0f)});
    }

    private void Update()
    {
        if (_hovering)
        {
/*            if (!_dontMagnifyUntilHoverAgainHack && !_myCard.Dragging)
            {
                if (transform.localScale.x < BaseScale.x * 1.5f)
                    transform.localScale += BaseScale * 5f * Time.deltaTime;
                if (transform.localScale.x > BaseScale.x * 1.5f)
                    transform.localScale = BaseScale * 1.5f;
            }*/

            _pointerOverTimer += Time.deltaTime;

            if (_myCard.Engine==null && !_myCard.Purchasable)
            {
                _glow.gameObject.SetActive(true);
                _glow.transform.localScale = _glowScale;
                if (!_glow.isPlaying) _glow.Play();
            }
            else if (_myCard.Engine != null && !_myCard.Purchasable)
            {
                _glow.transform.localScale = _glowScale *0.6f;
            }
            
            //generateCard
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
/*        var col = _glow.colorOverLifetime;
        if (_myCard.Engine!=null)
        {
            _glow.transform.localScale = _glowScale*0.4f;
            _glow.gameObject.SetActive(true);
            if (!_glow.isPlaying)
                _glow.Play();
        }
        else
        {
            col.color = _baseColor;
            _glow.transform.localScale = _glowScale;
        }*/
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
        
        //prevent when dragging card too fast it hover over card
        if (_myCard.InActive && _myCard.Engine != null) DeckManager.Instance.turnOffOtherRaycast(_myCard.MyIndex);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        CalcPosOnMouseMove();
        _myCard.MyCol.enabled = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        _myCard.Dragging = false;
        if (_myCard.Engine == null && _myCard._inSlot == true)
        {
            DeckManager.Instance.moveCardsToTray(_myCard.MyIndex, 0.3f);
        }
        
        if (_myCard.InActive && _myCard.Engine != null) DeckManager.Instance.turnOnRaycast();

        //I turn this off to make it so that it still scaled after you release the click
        //if(BaseScale != Vector3.zero)
        //transform.localScale = BaseScale;
        //_dontMagnifyUntilHoverAgainHack = true; 
    }

    private void CalcOffset()
    {
        _offset = transform.position -
                  new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
    }

    //Drag + Offset
    private void CalcPosOnMouseMove()
    {
        if(GameManager.Instance.State != GameState.Battle)
            return;
        if(_myCard.Engine != null && _myCard.Engine.EngineState == EngineState.Stacked)
            return;
        if(BattleManager.Instance.BattleState == BattleStates.BuyingCards)
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
            else if (_myCard.Engine != null && !_myCard.Purchasable && !_myCard.Dragging && Input.GetMouseButtonUp(1))
            {
                BattleManager.Instance.GetNextOpenEngine().RemoveCard(_myCard, true);
            }
        }

        if (GameManager.Instance.State == GameState.Customizing)
        {
            if(!_myCard.Equipped)
                CustomizeManager.Instance.SelectEquippedCard(_myCard);
            else
                CustomizeManager.Instance.DeselectEquipped(_myCard);
        }

        if (GameManager.Instance.State == GameState.Acquiring && !NewCardChooser.Instance.CardChosen)
        {
            NewCardChooser.Instance.ChooseCard(_myCard.CardName);
            Destroy(_myCard.gameObject);
        }
        
    }

    //Generate a card preview
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (BaseScale == Vector3.zero)
            BaseScale = transform.localScale;
        
        //Debug.Log(BaseScale);

        transform.DOScale(BaseScale*1.5f, 0.2f);
        
        //transform.position += Vector3.up * BaseScale.x * 2;
        if(GameManager.Instance.State == GameState.Battle)
            transform.SetSiblingIndex(transform.GetSiblingIndex() + 8);

        //show the engine that avaiable
        for (int i = 0; i < BattleManager.Instance.Engines.Length; i++)
        {
            if (BattleManager.Instance.Engines[i].PendingCount < 3) BattleManager.Instance.Engines[i].selectGear();
        }

        _hovering = true;
    }
    
    //Make card preview go away
    public override void OnPointerExit(PointerEventData eventData)
    {
        _pointerOverTimer = 0;

        if (BaseScale != Vector3.zero && !_myCard.Dragging)
        {
            transform.DOScale(BaseScale, 0.2f);
        }

        //fBaseScale = Vector3.zero;
        if (_myCard.Dragging == false)
        {
            _hovering = false;
            //show the engine that available
            foreach (var Engine in BattleManager.Instance.Engines)
            {
                Engine.disselectGear();
            }
        }
        Utils.DestroyCardPreview();
        _dontMagnifyUntilHoverAgainHack = false;

        if (_myCard.Engine == null)
        {
            var col = _glow.colorOverLifetime;
            //col.color = _baseColor;
            _glow.gameObject.SetActive(false);//_glow.Stop();
        }
        else
        {
            _glow.transform.localScale = _glowScale*0.4f;
            _glow.gameObject.SetActive(true);
            if (!_glow.isPlaying)
                _glow.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_myCard.InActive)
        {
            if (other.gameObject.CompareTag("CardPos"))
            {
                DeckManager DM = DeckManager.Instance;
                int pos_Index = Array.IndexOf(DM._cardPositions, other.transform);
                if (_myCard.MyIndex != pos_Index)
                {
                    DM.swapCardLocation(_myCard.MyIndex, pos_Index);
                    _myCard.MyIndex = pos_Index;
                }
                else
                {
                    _myCard._inSlot = true;
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (_myCard.InActive)
        {
            if (other.gameObject.CompareTag("TabZone"))
            {
                if (_myCard.Engine == null) _myCard._inSlot = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_myCard.InActive)
        {
            if (other.gameObject.CompareTag("TabZone"))
            {
                _myCard._inSlot = false;
            }
        }
    }
}
