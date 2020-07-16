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
    public bool hovering; //conceptual state of hovering tied to mouse click and other effects
    private bool _isMouseOver; //literal state of hovering helping to prevent bugs
    public Vector3 BaseScale = Vector3.zero;
    public Vector3 OutEngineScale = Vector3.zero; //use this scale when being out of engine
    public Vector3 InEngineScale = Vector3.zero; //use this scale when being in engine
    private bool _dontMagnifyUntilHoverAgainHack = false;

    //particle stuff
    public ParticleSystem Glow;
    public Vector3 GlowScale = Vector3.zero;
    private Gradient _inEngineColor, _baseColor;

    private void Start()
    {
        _myCard = GetComponent<Card>();
        hovering = false;
        
        //set up all glow related variable
        Glow = transform.Find("PuffyGlow").GetComponent<ParticleSystem>();
        GlowScale = Glow.transform.localScale;

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
        if (hovering)
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
        else
        {
            if (BaseScale != Vector3.zero)
            {
                if (!_myCard.Dragging && _myCard.Engine == null && transform.localScale != OutEngineScale) //recheck if card suoppose to be outside of engine size with aura off
                {
                    transform.DOScale(OutEngineScale, 0.2f);
                    _myCard.SwitchTypeAura(false);
                }
                else if (!_myCard.Dragging && _myCard.Engine != null && transform.localScale != InEngineScale) //recheck if the card suppose to be in engine size with aura on
                {
                    transform.DOScale(InEngineScale, 0.2f);
                    _myCard.SwitchTypeAura(true);
                }   
            }
        }

        /*if (transform.localScale.x == 0)
        {
            transform.localScale = Vector3.one * 30;
        }*/
    }
    #endregion

    #region Mouse Click Event
    public override void OnPointerDown(PointerEventData eventData)
    {
        if(GameManager.Instance.State == GameState.Battle)
        {
            BattleManager.Instance.isMouseDragging = true; //make mouseDagFalse

            if (_myCard.Engine != null && _myCard.Engine.EngineState == EngineState.Stacked && BattleManager.Instance.BattleState == BattleStates.ChoosingAction)
                return; //if this is choosing action state, dont do any shit;
            
            if (_myCard.Purchasable)
            {
                BuyManager.Instance.BuyCard(_myCard);
                BattleManager.Instance.isMouseDragging = false;
            }
            else
            {
                CalcOffset();
                _myCard.Dragging = true;

                //put it in front of everything again if got click in engine
                if (_myCard.Engine != null)
                {
                    transform.SetParent(DeckManager.Instance.transform.parent); //put in front
                    transform.DOScale(OutEngineScale * 1.5f, 0.2f); //change size to normal hover
                    setParticleGlowSize(1f);//change particle saize to normal hover
                }
                
                //prevent when dragging card too fast it hover over card
                if (_myCard.InActive)
                {
                    DeckManager.Instance.turnOffOtherRaycast(_myCard.MyIndex);
                }
            }
        }

        if (GameManager.Instance.State == GameState.Customizing)
        {
            //Check if in deck, if not add to deck and add highlight
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        CalcPosOnMouseMove();
        _myCard.MyCol.enabled = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {

        if (GameManager.Instance.State == GameState.Battle)
        {
            BattleManager.Instance.isMouseDragging = false; //make mnouseDrag false
            
            if (_myCard.Engine != null && _myCard.Engine.EngineState == EngineState.Stacked && BattleManager.Instance.BattleState == BattleStates.ChoosingAction)
                return; //if this is choosing action state, dont do any shit;
            
            _myCard.Dragging = false;
            if (_myCard.Engine == null && _myCard._inSlot)
            {
                DeckManager.Instance.moveCardToTray(_myCard.MyIndex, 0.2f);
                if (_myCard.InActive) DeckManager.Instance.turnOnRaycast(); //turn on raycast other card raycast
            }
            else if (_myCard.Engine != null)
            {
                //move card back to Enigne Slot
                transform.DOScale(BaseScale * 1.5f, 0.1f);
                _myCard.Engine.moveCardToEngineSlot(_myCard.MyEngineIndex, 0.2f);
                setParticleGlowSize(0.37f);
            }
            else
            {
                if (_myCard.InActive) DeckManager.Instance.turnOnRaycast(); //turn on raycast other card raycast
            }    

            //change size of card in case of fast hovering
            if (!_isMouseOver && hovering)
            {
                hovering = false;
                transform.DOScale(BaseScale, 0.2f);
            }
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
            if (_myCard.Engine != null && _myCard.Engine.EngineState == EngineState.Stacked && BattleManager.Instance.BattleState == BattleStates.ChoosingAction && Input.GetMouseButtonUp(0))
            {
                _myCard.Engine.Select();
            }
            else if (_myCard.Engine == null && !_myCard.Purchasable && !_myCard.Dragging && Input.GetMouseButtonUp(1))
            {
                //Find an empty engine
                BattleManager.Instance.GetNextOpenEngine().AddCard(_myCard);
                
                //trun off Engine highlight after assign engine
                foreach (var Engine in BattleManager.Instance.Engines)
                {
                    Engine.disselectGear();
                }
                
            }
            else if (_myCard.Engine != null && !_myCard.Purchasable && !_myCard.Dragging && Input.GetMouseButtonUp(1))
            {
                //BattleManager.Instance.GetNextOpenEngine().RemoveCard(_myCard, true);
                _myCard.Engine.RemoveCard(_myCard, true);
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
        if (BaseScale == Vector3.zero) //set all the scale the first time pointer touch the card
        {
            BaseScale = transform.localScale;
            OutEngineScale = BaseScale;
            InEngineScale = BaseScale * 0.7f;
        }

        //if not actually still holding it, make it bigger
        if (!_myCard.Dragging) transform.DOScale(BaseScale*1.5f, 0.2f);
        
        //do whatever important in battel
        if (GameManager.Instance.State == GameState.Battle)
        {
            transform.SetSiblingIndex(transform.GetSiblingIndex() + 8);

            //show the engine that available
            if (!_myCard.Dragging && !_myCard.Purchasable && _myCard.Engine == null)
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
            
            Glow.gameObject.SetActive(true);
            setParticleGlowSize(1f);
            if (!Glow.isPlaying) Glow.Play();
        }
        else if (_myCard.Engine != null && !_myCard.Purchasable)
        {
            setParticleGlowSize(0.65f); //make hovering glow size
        }

        //change hovering
        hovering = true;
        
        AudioManager.Instance.PlayClip("hover");
        
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
            hovering = false;
            //show the engine that available
            if (GameManager.Instance.State == GameState.Battle
                && BattleManager.Instance.BattleState != BattleStates.ChoosingAction
                && _myCard.Engine == null)
            {
                foreach (var Engine in BattleManager.Instance.Engines)
                {
                    Engine.disselectGear();
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
            Glow.gameObject.SetActive(false);//_glow.Stop();
            _myCard.SwitchTypeAura(false); //turn off Type Aura
        }
        else if (_myCard.Engine != null && !_myCard.Purchasable)
        {
            if (_myCard.Engine._highlighted) setParticleGlowSize(0.45f);
            else setParticleGlowSize(0.37f);//make glow in engine normal size
            
            if (!Glow.gameObject.activeSelf) Glow.gameObject.SetActive(true); //if particle is off, turn it fucking on
            if (!Glow.isPlaying) Glow.Play(); //if its not playing, make it play
        }
    }
    #endregion

    #region Collision Event
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_myCard.InActive && !_myCard.Purchasable)
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

            //when touch with engine
            if (_myCard.Dragging && other.gameObject.CompareTag("Engine"))
            {
                Engine thisEngine = other.GetComponent<Engine>();
                
                if (_myCard.pendingEngine == null) //if there no engine in pending before, turn it on
                {
                    _myCard.pendingEngine = thisEngine;
                    _myCard.pendingEngine.highlightedOn();
                    _myCard.pendingEngine.attackOnPositionPreviewOn();
                }
                else if (thisEngine != _myCard.pendingEngine) //if there was an engine in pending but it's different one
                {
                    //turn off old engine
                    _myCard.pendingEngine.highlightedOff();
                    _myCard.pendingEngine.attackOnPositionPreviewOff();
                    if (_myCard.Engine == _myCard.pendingEngine) _myCard.pendingEngine.RemoveCard(_myCard, false); //remove if was in the engine
                    
                    //turn on new engine
                    _myCard.pendingEngine = thisEngine;
                    _myCard.pendingEngine.highlightedOn();
                    _myCard.pendingEngine.attackOnPositionPreviewOn();
                    
                    //show available engine
                    if (BattleManager.Instance.BattleState != BattleStates.ChoosingAction && _myCard.pendingEngine == null)
                    {
                        for (int i = 0; i < BattleManager.Instance.Engines.Length; i++)
                        {
                            if (BattleManager.Instance.Engines[i].PendingCount < 3) BattleManager.Instance.Engines[i].selectGear();
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (_myCard.InActive && !_myCard.Purchasable)
        {
            if (other.gameObject.CompareTag("TabZone"))
            {
                if (_myCard.Engine == null && _myCard.pendingEngine == null) _myCard._inSlot = true;
                
                //use this to make sure the card will back to its place
                if (!_event_inSlot && _myCard._inSlot && _myCard.Engine == null && !_myCard.Dragging)
                {
                    DeckManager.Instance.moveCardToTray(_myCard.MyIndex, 0.3f);
                    transform.DOScale(OutEngineScale, 0.2f);
                    _event_inSlot = true;
                }
            }

            if (_myCard.Dragging && other.gameObject.CompareTag("Engine"))
            {
                if (_myCard.pendingEngine == null) //if just gert out of other engine but still stay in the old one for some reason
                {
                    _myCard.pendingEngine = other.GetComponent<Engine>();
                    _myCard.pendingEngine.highlightedOn();
                    _myCard.pendingEngine.attackOnPositionPreviewOn();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_myCard.InActive && !_myCard.Purchasable)
        {
            if (other.gameObject.CompareTag("TabZone"))
            {
                _myCard._inSlot = false;
                _event_inSlot = false;
            }
            
            if (_myCard.Dragging && other.gameObject.CompareTag("Engine"))
            {
                Engine thisEngine = other.GetComponent<Engine>();
                
                if (thisEngine == _myCard.pendingEngine) //if get out of the one being pended
                {
                    _myCard.pendingEngine.highlightedOff();
                    _myCard.pendingEngine.attackOnPositionPreviewOff();
                    
                    _myCard.pendingEngine = null; //then get rid of it
                }

                //show available engine
                if (BattleManager.Instance.BattleState != BattleStates.ChoosingAction && _myCard.pendingEngine == null)
                {
                    for (int i = 0; i < BattleManager.Instance.Engines.Length; i++)
                    {
                        if (BattleManager.Instance.Engines[i].PendingCount < 3) BattleManager.Instance.Engines[i].selectGear();
                    }
                }
            }
        }
    }
    #endregion

    #region referenceFunctions

    public void setParticleGlowSize(float scaler)
    {
        Glow.transform.DOScale(GlowScale * scaler , 0.3f);
    }

    #endregion
}
