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

    private bool _event_inSlot = true; //use this variable to double check if the card suppose to be in slot or not

    //Card Previews
    private float _pointerOverTimer;
    public bool _hovering; //conceptual state of hovering tied to mouse click and other effects
    private bool _isMouseOver; //literal state of hovering helping to prevent bugs
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
        
        #region set color of Particle

        /*Color whiteSmoke = new Color(0.5f,0.8f,1);
        _baseColor = new Gradient();
        _baseColor.SetKeys(new GradientColorKey[]{new GradientColorKey(whiteSmoke, 0.0f)}, 
            new GradientAlphaKey[]{new GradientAlphaKey(0.0f, 0.0f), new GradientAlphaKey(1.0f, 0.524f), new GradientAlphaKey(0.0f, 1.0f)});*/

        #endregion
    }

    #region update event
    private void Update()
    {
        if (_hovering)
        {
            _pointerOverTimer += Time.deltaTime;

            //generateCard
            if (!_myCard.ShowFullSize) Utils.GenerateCardPreview(_myCard);
            
            //for hotkey to put in engine
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

        /*if (transform.localScale.x == 0)
        {
            transform.localScale = Vector3.one * 30;
        }*/
        
        //use this to make sure the card will back to its place
        if (!_event_inSlot && _myCard._inSlot)
        {
            DeckManager.Instance.moveCardsToTray(_myCard.MyIndex, 0.3f);
            _event_inSlot = true;
        }
    }
    #endregion

    #region Mouse Click Event
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
        if (_myCard.InActive && _myCard.Engine == null)
        {
            DeckManager.Instance.turnOffOtherRaycast(_myCard.MyIndex);
        }
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

        //turn on raycast other card raycast
        if (_myCard.InActive && _myCard.Engine == null) DeckManager.Instance.turnOnRaycast();
        
        //change size of card in case of fast hovering
        if (!_isMouseOver && _hovering)
        {
            _hovering = false;
            transform.DOScale(BaseScale, 0.2f);
        }
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
    #endregion
    
    #region Mouse Over Event
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (BaseScale == Vector3.zero)
            BaseScale = transform.localScale;

            //if not actually still holding it, make it bigger
        if (!_myCard.Dragging) transform.DOScale(BaseScale*1.5f, 0.2f);
        
        //do whatever important in battel
        if (GameManager.Instance.State == GameState.Battle)
        {
            transform.SetSiblingIndex(transform.GetSiblingIndex() + 8);

            //show the engine that available
            if (!_myCard.Dragging && !_myCard.Purchasable)
            {
                if (BattleManager.Instance.BattleState != BattleStates.ChoosingAction)
                {
                    for (int i = 0; i < BattleManager.Instance.Engines.Length; i++)
                    {
                        if (BattleManager.Instance.Engines[i].PendingCount < 3) BattleManager.Instance.Engines[i].selectGear();
                    }
                }
            }
        }
        
        //for particle and effect
        if (_myCard.Engine==null && !_myCard.Purchasable)
        {
            _myCard.SwitchTypeAura(true); //turn on Type Aura
            
            _glow.gameObject.SetActive(true);
            _glow.transform.DOScale(_glowScale, 0.2f);
            if (!_glow.isPlaying) _glow.Play();
        }
        else if (_myCard.Engine != null && !_myCard.Purchasable)
        {
            _glow.transform.DOScale(_glowScale*0.65f, 0.2f);
        }

        //change hovering
        _hovering = true;
        
        //change mouse over
        _isMouseOver = true;
    }

    //when move mouse away
    public override void OnPointerExit(PointerEventData eventData)
    {
        _pointerOverTimer = 0;

        if (BaseScale != Vector3.zero && !_myCard.Dragging)
        {
            transform.DOScale(BaseScale, 0.2f);
        }

        //change hovering
        if (!_myCard.Dragging && !_myCard.Purchasable) //prevent 
        {
            _hovering = false;
            //show the engine that available
            if (GameManager.Instance.State == GameState.Battle)
            {
                if (BattleManager.Instance.BattleState != BattleStates.ChoosingAction)
                {
                    foreach (var Engine in BattleManager.Instance.Engines)
                    {
                        Engine.disselectGear();
                    }
                }
            }
        }
        
        //change mouse over
        _isMouseOver = false;
        
        if (!_myCard.ShowFullSize) Utils.DestroyCardPreview(); //Make card preview go away
        _dontMagnifyUntilHoverAgainHack = false;

        //turn on/off particle based on being in engine or not
        if (_myCard.Engine == null && !_myCard.Purchasable)
        {
            _glow.gameObject.SetActive(false);//_glow.Stop();
            _myCard.SwitchTypeAura(false); //turn off Type Aura
        }
        else if (_myCard.Engine != null && !_myCard.Purchasable)
        {
            _glow.transform.DOScale(_glowScale*0.37f, 0.2f);
            _glow.gameObject.SetActive(true);
            if (!_glow.isPlaying)
                _glow.Play();
        }
    }
    #endregion

    #region Collision Event
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
                _event_inSlot = false;
            }
        }
    }
    #endregion
}
