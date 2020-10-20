using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class Engine : MonoBehaviour
{
    public Stack<Card> Stack;
    
    //UI
    public Color GlowColor;
    private Animator u_Circle;
    private Material u_CircleGlowMat; 
    private GameObject u_EngineImg;
    private Animator u_EngineImgAnim;
    private bool _wheelTurning;
    private Tooltip _tooltip;
    private RectTransform[] _cardPositons;

    private TMP_Text u_powerNumber;
    private Image u_powerCore;
    private TMP_Text u_aetherNumber;
    private Image u_aetherCore;
    private Image[] u_move;
    private Image u_selectedAura;
    private Image[] u_slotFilledAura;
    
    private Transform[] u_AttackOnPosition = new Transform[3];
    private TMP_Text[] u_AttackOnPosNumber = new TMP_Text[3];

    //animator
    private Animator slotAuraAnim;

    //for state change event
    [HideInInspector] public Vector3 _baseScale;
    private GameObject _myCheatImage;
    private GameObject _myCheatImageForNewRayCastCheck;
    private Image _blackMask;
    private Vector3 _originalScale;
    [Range(1, 3)] public int engineNumber = 1;
    [HideInInspector] public List<Transform> _statePos;

    [SerializeField] private List<Card> _pending;
    private Vector3 _initialPosition;
    private BoxCollider2D _collider;

    [HideInInspector] public bool _selected;
    [HideInInspector] public bool _highlighted;

    public bool isActive;

    //for pointer event
    private GameObject _gear;
    private Animator _gearAnim;
    private bool blockAura;

    //Game Vars
    //Total attack this round
    private int _powerTotal;
    public int PowerTotal
    {
        get => _powerTotal;
        set => _powerTotal = value;
    }

    private int[] _attackPower = new int[3];
    
    //Total aether this round
    private int _aetherTotal;
    public int AetherTotal
    {
        get => _aetherTotal;
        set => _aetherTotal = value;
    }

    //Total movement this round
    private int _moveTotal;
    public int MoveTotal
    {
        get => _moveTotal;
        set => _moveTotal = value;
    }

    [HideInInspector] public int OverridePower, OverrideAether, OverrideMove = -1;
    
    public int AmtMoved;
    
    private bool _inRange = true;
    
    public EngineState EngineState;

    public bool EmptyStack = false;
    public bool Executed;
    private void Awake()
    { 
        Init();
    }

    private void Init()
    {
        Stack = new Stack<Card>();
        _pending = new List<Card>();
        EngineState = EngineState.Stacking;
        _initialPosition = transform.position;
        Executed = false;
        _collider = GetComponent<BoxCollider2D>();
        
        u_Circle = GetComponent<Animator>();
        //u_CircleGlowMat = Instantiate(u_Circle.GetComponent<Image>().material);
        //u_Circle.GetComponent<Image>().material = u_CircleGlowMat;
        u_selectedAura = transform.Find("SelectedAura").GetComponent<Image>();
        u_slotFilledAura = transform.Find("CardSlot_FilledAura").GetComponentsInChildren<Image>();

        Transform posPanel = transform.parent.transform.parent.transform.Find("PositionsPanel").transform.Find("AttackRange");
        u_AttackOnPosition[0] = posPanel.transform.Find("Range_1");
        u_AttackOnPosNumber[0] = u_AttackOnPosition[0].GetComponentInChildren<TMP_Text>();
        u_AttackOnPosition[1] = posPanel.transform.Find("Range_2");
        u_AttackOnPosNumber[1] = u_AttackOnPosition[1].GetComponentInChildren<TMP_Text>();
        u_AttackOnPosition[2] = posPanel.transform.Find("Range_3");
        u_AttackOnPosNumber[2] = u_AttackOnPosition[2].GetComponentInChildren<TMP_Text>();

        //u_EngineImg = transform.Find("EngineImg").gameObject;
        //u_EngineImgAnim = u_EngineImg.GetComponent<Animator>();

        _tooltip = null;
        
        _cardPositons = transform.Find("CardPositions").GetComponentsInChildren<RectTransform>();
        
        //set up the position to go in each state
        Transform[] _stateTran= transform.parent.transform.Find("EnginePos" + engineNumber).GetComponentsInChildren<Transform>();
        for (int i = 1; i < _stateTran.Length; i++)
        {
            _statePos.Add(_stateTran[i]);
        }
        

        //set up images and cheatImg for state change
        _baseScale = transform.localScale;
        _myCheatImage = transform.Find("CheatImg").gameObject;
        _myCheatImageForNewRayCastCheck = transform.parent.transform.Find("BackCheatImage").gameObject;
        _blackMask = transform.Find("Black_Mask").GetComponent<Image>();
        _originalScale = transform.localScale;

        //anim
        slotAuraAnim = transform.Find("SlotAura").GetComponent<Animator>();
        _gear = transform.Find("GearAnim").transform.Find("Gear").gameObject;
        _gearAnim = transform.Find("GearAnim").GetComponent<Animator>();
        //set Gear Sprite 
        Image engineNumGear = _gear.transform.Find("EngineNumber").GetComponent<Image>();
        engineNumGear.sprite = Resources.Load<Sprite>("Sprites/Engine" + engineNumber);

        //set up number and Icon for total engine power
        u_powerNumber = transform.Find("PowerNumber").GetComponent<TMP_Text>();
        u_powerCore = transform.Find("AttackEngine").transform.Find("Core_Main").GetComponent<Image>();
        u_aetherNumber = transform.Find("AetherNumber").GetComponent<TMP_Text>();
        u_aetherCore = transform.Find("AetherEngine").transform.Find("Core_Main").GetComponent<Image>();
        u_move = transform.Find("MoveIcon").GetComponentsInChildren<Image>();
        
        OverridePower = OverrideAether = OverrideMove = -1;
    }
    
    //start
    void Start()
    {
        StateChange(0);
    }

    //Adds a card to the pending card array
    public void AddCard(Card c)
    {
        if(c.Engine == this)
            return;

        if (PendingCount >= 3)
        {
            DeckManager.Instance.moveCardToTray(c.MyIndex, 0.5f);
            return;
        }

        if (c.Engine != null)
            c.Engine.RemoveCard(c, false);

        c.Engine = this;
        c.SetEngine(_cardPositons[0].parent.transform, CurrentCardPos(_pending.Count));
        c.MyEngineIndex = _pending.Count;
        _pending.Add(c);
        DeckManager.Instance.CardsToBeSorted.Remove(c);
        UpdateUICounts();
        
        //see if all card is there 
        if (EngineState == EngineState.Stacking && _pending.Count >= 3) StateChange(2);

            //turn on the circle
        MagicCircle();
        //update slot light
        updateSlotFilled();
    }

    public void RemoveCard(Card c, bool isClick)
    {
        int cInd = _pending.IndexOf(c);
        Card nextC = null;
        
        if(_pending.Count > 1 && cInd != _pending.Count - 1)
            nextC = _pending[cInd + 1];
        
        //see if all card is there 
        if (EngineState == EngineState.Stacking && _pending.Count >= 3) StateChange(1);
        
        _pending.Remove(c);
        DeckManager.Instance.CardsToBeSorted.Add(c);
        c.OffEngine(DeckManager.Instance.transform.parent);
        c.Engine = null;
        if (isClick)
            DeckManager.Instance.moveCardToTray(c.MyIndex,0.5f);

        //turn off magic circle
        MagicCircle();
        //update slot light
        updateSlotFilled();
        UpdateUICounts();
        if(cInd == _pending.Count) //if this is the last card
            return;
        if(_pending.Count == 0) //if there is no card left
            return;
        
        nextC.transform.localPosition = CurrentCardPos(cInd);
        nextC.MyEngineIndex = cInd;
        while (cInd < _pending.Count-1)
        {
            cInd++;
            nextC = _pending[cInd];
            nextC.transform.DOLocalMove(CurrentCardPos(cInd), 0.1f);
        }
    }

    public void ReadyCards()
    {
        if (EngineState == EngineState.Stacking)
        {
            foreach (var c in _pending)
            {
                c.ReadyCard();
            }
        }
        else
        {
            foreach (var c in Stack)
            {
                c.ReadyCard();
            }
        }
    }
    
    public void StackCards()
    {
        if (EngineState != EngineState.Stacked)
        {
            ToggleMode();
        }

        if (_pending.Count == 0)
        {
            EmptyStack = true;
            return;
        }

        int lowest = Int32.MaxValue;
        int indexToStack = 0;
        
        for(int i = 0; i < _pending.Count; i++)
        {
            int currentPriority = _pending[i].Priority;
            if (currentPriority < lowest)
            {
                lowest = currentPriority;
                indexToStack = i;
            }
        }

        Card currentCard = _pending[indexToStack];
        
        currentCard.transform.SetSiblingIndex(Stack.Count);
        //currentCard.transform.localPosition = CurrentCardPos(Stack.Count);
        Stack.Push(currentCard);
        _pending.RemoveAt(indexToStack);
        
        if(_pending.Count > 0)
            StackCards();
        
        UpdateUICounts();
    }
    
    public void StackCardsForPreview()
    {
        int lowest = Int32.MaxValue;
        int indexToStack = 0;
        
        for(int i = 0; i < _pending.Count; i++)
        {
            int currentPriority = _pending[i].Priority;
            if (currentPriority < lowest)
            {
                lowest = currentPriority;
                indexToStack = i;
            }
        }

        if(_pending.Count == 0)
            return;

        Card currentCard = _pending[indexToStack];
        Stack.Push(currentCard);
        _pending.RemoveAt(indexToStack);
        
        if(_pending.Count > 0)
            StackCardsForPreview();
    }

    public List<Card> PoppedCards; //Cards go here between execution steps 1 and 2
    public IEnumerator ExecuteStack()
    {
        BattleDelegateHandler.ApplyEngineEffects();
        ReadyCards();
        PoppedCards = new List<Card>();
        while (Stack.Count > 0)
        {
            Card c = Stack.Pop();
            if (c.IsXCost) //-1 is X
            {
                XDialog xd = Instantiate(Resources.Load<GameObject>("prefabs/xdialog"), GameObject.Find("MainCanvas").transform).GetComponent<XDialog>();
                xd.AetherMax = _aetherTotal;
                //Wait until user assigns x value
                yield return new WaitUntil(() => xd.XConfirmed);
                _aetherTotal -= xd.XValue;
                c.XValue = xd.XValue;
                Destroy(xd.gameObject);
                c.Execute();
            } 
            else if (c.PayForCard())
            {
                c.Execute();
            }
            else
            {
                Utils.DisplayError("Not enough aether to power " + c.CardName + "!", 0.5f);
                c.ExecuteFailed();
            }
            _aetherTotal += c.AetherTotal;
            _moveTotal += c.MoveTotal;
            PoppedCards.Add(c);
        }

        if (OverrideAether != -1)
            _aetherTotal = OverrideAether;
        if (OverrideMove != -1)
            _moveTotal = OverrideMove;
        
        //Stat Check! -only move implemented
        if (_moveTotal > 0)
        {
            var playerStats = BattleManager.Instance.Player.ActiveStatsList;
            _moveTotal = StatManager.Instance.StatCheck(_moveTotal, playerStats, StatType.MovesUP, StatType.MovesDOWN);
        }

        if (_moveTotal > 3) _moveTotal = 3; //make sure it never go above 3
        if (_moveTotal < 0) _moveTotal = 0; //make i=sure it never go under 0
        
        Executed = true;
        EmptyStack = false;
        Deselect();
    }

    //Last stage of execution
    public void CalculatePowerTotal()
    {
        foreach (var c in PoppedCards)
        {
            _powerTotal += c.CalculateAttackTotalWithPosition();
            if (OverridePower != -1)
                _powerTotal = OverridePower;
            //Finish Executing
            if (c.TrashThis)
                DeckManager.Instance.Trash(c);
            else
                DeckManager.Instance.Discard(c);
        }
        
        //Stat Check
        if (_powerTotal > 0)
        {
            var playerStats = BattleManager.Instance.Player.ActiveStatsList;
            _powerTotal =
                StatManager.Instance.StatCheck(_powerTotal, playerStats, StatType.AttackUP, StatType.AttackDOWN);
        }
        
        MagicCircle();
    }

    private int ExecuteStackForPreview()
    {
        int totalCost = 0;

        List<Card> tempPending = new List<Card>(_pending);

        if (EngineState == EngineState.Stacking)
            StackCardsForPreview();

        _attackPower = new int[3];

        foreach (var c in Stack)
        {
            if (c.IsXCost) //-1 is X
            {
                continue;
            }
            
            //will fix this later
            if (c.StatMods.Count <= 0) c.Execute(); 
            
            _aetherTotal += c.AetherTotal;
            _moveTotal += c.MoveTotal;
            totalCost += c.AetherCost;
            if (!c.IsAttackInRange() && _inRange)
                _inRange = false;
        }

        foreach (var c in Stack)
        {
            _powerTotal += c.CalculateAttackTotalWithPosition();
            switch (c.Range)
            {
                case AttackRange.Melee:
                    _attackPower[0] += c.PowerTotal;
                    break;
                case AttackRange.Short:
                    _attackPower[0] += c.PowerTotal;
                    _attackPower[1] += c.PowerTotal;
                    _attackPower[2] += (int) (c.PowerTotal * 0.5f);
                    break;
                case AttackRange.Long:
                    _attackPower[1] += c.PowerTotal;
                    _attackPower[2] += c.PowerTotal;
                    break;
            }
        }

        //add buff and debuff
        var playerStats = BattleManager.Instance.Player.ActiveStatsList;
        Stat s;
        
        //attack
        if (_powerTotal > 0) _powerTotal = StatManager.Instance.StatCheck(_powerTotal, playerStats, StatType.AttackUP, StatType.AttackDOWN);
        if (_attackPower[0] > 0) _attackPower[0] = StatManager.Instance.StatCheck(_attackPower[0], playerStats, StatType.AttackUP, StatType.AttackDOWN);
        if (_attackPower[1] > 0) _attackPower[1] = StatManager.Instance.StatCheck(_attackPower[1], playerStats, StatType.AttackUP, StatType.AttackDOWN);
        if (_attackPower[2] > 0) _attackPower[2] = StatManager.Instance.StatCheck(_attackPower[2], playerStats, StatType.AttackUP, StatType.AttackDOWN);

        //if attack is less than zero then set it to be zero
        if (_powerTotal < 0) _powerTotal = 0;
        
        for (int i = 0; i < _attackPower.Length; i++)
        {
            if (_attackPower[i] < 0) _attackPower[i] = 0;
        }
        
        //speed
        if (_moveTotal > 0) _moveTotal = StatManager.Instance.StatCheck(_moveTotal, playerStats, StatType.MovesUP, StatType.MovesDOWN);

        //make sure speed not go over 3 and under0
        if (_moveTotal > 3) _moveTotal = 3;
        else if (_moveTotal < 0) _moveTotal = 0;

        //check if engine state is to clear stack or not
        if (EngineState == EngineState.Stacking)
        {
            _pending = tempPending;
            Stack.Clear();
        }

        return totalCost;
    }
    
    public void ShowPreview()
    {
        //u_CircleGlowMat.SetColor("_MyColor", Color.yellow);
        
        string previewStr = "";
        int tempPow, tempAet, tempMove, tempCost;
        tempPow = tempAet = tempMove = tempCost = 0;
        bool tempInRange;
        if (EngineState == EngineState.Stacking)
        {
            if(_pending.Count == 0)
                return;
        }
        
        //Clone this engine, and calculate
        ReadyCards();
        tempCost = ExecuteStackForPreview();

        tempPow = PowerTotal;
        _powerTotal = 0;
        tempAet = AetherTotal - tempCost;
        _aetherTotal = 0;
        tempMove = MoveTotal;
        _moveTotal = 0;
        tempInRange = _inRange;
        _inRange = true;
        
        previewStr = "";
        if(tempPow > 0)
            previewStr += tempPow + " Power" + "\n";
        if (tempAet > 0)
            previewStr += tempAet + " Aether" + "\n";
        if (tempMove > 0)
            previewStr += tempMove + " Move" + "\n";
        if (tempCost > 0)
            previewStr += "Cards in this engine will cost " + tempCost + " total Aether to use.";
        if (!tempInRange)
            previewStr += "Some cards in this engine are not in range!";
        
        _tooltip = Instantiate(Resources.Load<GameObject>("prefabs/tooltip"), _gear.transform.position,
            Quaternion.identity, _gear.transform).GetComponent<Tooltip>();
        _tooltip.Text = previewStr;
    }

    public void HidePreview()
    {
        //if(!_selected)
            //u_CircleGlowMat.SetColor("_MyColor", GlowColor);
        if(_tooltip == null)
            return;
        _tooltip.StartCoroutine(_tooltip.FadeOut());
        _tooltip = null;
    }
    
    //Collisions
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Card"))
            return;
        Card c = other.gameObject.GetComponent<Card>();
        if (EngineState != EngineState.Stacking)
            return;

        if (c.Engine != null && c.Engine.EngineState == EngineState.Stacked || c.Purchasable || c.Dragging || c.Tweening || c.IsPreview || c.pendingEngine != this)
            return;
        AddCard(c);
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        OnTriggerEnter2D(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Card"))
            return;
        Card c = other.gameObject.GetComponent<Card>();
        
        if (c.Engine != this || !c.Dragging) //if the card that gets out is not the belong to this engine, do no shit
            return;
        
        RemoveCard(c, false);
    }

    public int PendingCount
    {
        get => _pending.Count;
    }
    
    private void Update()
    {
        if ((PendingCount >= 3 || (EngineState == EngineState.Stacked && Stack.Count != 0)) && !_wheelTurning)
        {
            //u_EngineImgAnim.SetBool("IsReady", true);
            /*if(_steamParticle.isPlaying)
                _steamParticle.Stop();
            _steamParticle.Play();
            
            if(_steamBubble.isPlaying)
                _steamBubble.Stop();
            _steamBubble.Play();*/
            
            _wheelTurning = true;
            
            u_Circle.SetBool("TurnOn", true);
        }
        else if (PendingCount < 3 && _wheelTurning && EngineState != EngineState.Stacked || (EngineState == EngineState.Stacked && Stack.Count == 0))
        {
           // u_EngineImgAnim.SetBool("IsReady", false);
            //_steamParticle.Stop();
            _wheelTurning = false;
            u_Circle.SetBool("TurnOn", false);
        }
        
        /*if (Input.GetKeyDown(KeyCode.A)) turnedEngineOff();
        if (Input.GetKeyDown(KeyCode.S)) prepareEngine();
        if (Input.GetKeyDown(KeyCode.D)) turnOnEngine();*/
    }

    public void UpdateUICounts(bool setToZero = false)
    {
        for (int i = 0; i < u_move.Length; i++)
        {
            u_move[i].color = new Color(0,0,0,0);
        }
        
        int tempPow, tempAet, tempMove, tempCost;
        tempPow = tempAet = tempMove = tempCost = 0;
        bool tempInRange = false;
        
        //Clone this engine, and calculate
        ReadyCards();
        if (_pending.Count != 0 || Stack.Count != 0)
            tempCost = ExecuteStackForPreview();

        tempPow = PowerTotal;
        _powerTotal = 0;
        tempAet = AetherTotal - tempCost;
        _aetherTotal = 0;
        tempMove = MoveTotal;
        _moveTotal = 0;
        tempInRange = _inRange;
        _inRange = true;
        
        
        if (setToZero)
        {
            tempPow = tempAet = tempMove = tempCost = 0;

            for (int i = 0; i < _attackPower.Length; i++)
            {
                _attackPower[i] = 0;
            }
        }

        //set TotalPower
        u_powerNumber.text = tempPow.ToString();
        u_aetherNumber.text = tempAet.ToString();
        for (int i = 0; i < tempMove; i++)
        {
            u_move[i].color = Color.white;
        }

        if (tempPow > 0)
        {
            u_powerNumber.font = Resources.Load<TMP_FontAsset>("Fonts/Palmmy/Bebsa_SDF_PowerGlow");
            u_powerNumber.color = Color.white;
            
            u_powerCore.sprite = Resources.Load<Sprite>("Sprites/Core/AttackCore_On");
            u_powerCore.SetNativeSize();
        }
        else
        {
            u_powerNumber.font = Resources.Load<TMP_FontAsset>("Fonts/Palmmy/BebasNeue-Regular SDF");
            u_powerNumber.color = new Color(0.4f,0.4f, 0.4f,1);
            
            u_powerCore.sprite = Resources.Load<Sprite>("Sprites/Core/AttackCore_Off");
            u_powerCore.SetNativeSize();
        }

        if (tempAet > 0)
        {
            u_aetherNumber.font = Resources.Load<TMP_FontAsset>("Fonts/Palmmy/Bebsa_SDF_AetherGlow");
            u_aetherNumber.color = Color.white;
            
            u_aetherCore.sprite = Resources.Load<Sprite>("Sprites/Core/ManaCore_On");
            u_aetherCore.SetNativeSize();
        }
        else
        { 
            u_aetherNumber.font = Resources.Load<TMP_FontAsset>("Fonts/Palmmy/BebasNeue-Regular SDF");
            u_aetherNumber.color = new Color(0.4f,0.4f, 0.4f,1);
            
            u_aetherCore.sprite = Resources.Load<Sprite>("Sprites/Core/ManaCore_Off");
            u_aetherCore.SetNativeSize();
        }
        
        //u_rarity.sprite = Resources.Load<Sprite>("Sprites/Rarity_Common");
    }

    public void ToggleMode()
    {
        if (EngineState == EngineState.Stacking)
        {
            EngineState = EngineState.Stacked;
            foreach (Card c in Stack)
            {
                c.GetComponent<Collider2D>().enabled = false;
            }
        }
        else
        {
            EngineState = EngineState.Stacking;
            Executed = false;
            _aetherTotal = 0;
            _powerTotal = 0;
            _moveTotal = 0;
            AmtMoved = 0;
            _inRange = true;
            OverrideAether = OverrideMove = OverrideAether = -1;
            EmptyStack = false;
        }
    }
    
    public void Deselect()
    {
        /*foreach (Card c in Stack)
        {
            c.SetEngine(GlowColor, u_Circles[1].transform);
        }*/
        //u_CircleGlowMat.SetColor("_MyColor", GlowColor);
        _selected = false;
        disselectGear();
    }

    public void Select()
    {
        /*foreach (Card c in Stack)
        {
            c.SetEngine(Color.white, u_Circles[1].transform);
        }*/
        if(EngineState != EngineState.Stacked || BattleManager.Instance.BattleState != BattleStates.ChoosingAction || (Stack.Count == 0 && !EmptyStack))
            return;
        //u_CircleGlowMat.SetColor("_MyColor", Color.yellow);
        BattleManager.Instance.EngineSelected();
        BattleManager.Instance.PushAttack(this);
        _selected = true;
    }

    private void OnMouseEnter()
    {
//        Debug.Log("over");
    }

    private Vector3 CurrentCardPos(int count)
    {
        return _cardPositons[count].localPosition;
    }

    private void MagicCircle()
    {
        if (PendingCount >= 3) u_Circle.SetBool("TurnOn", true);
        else
        {
            u_Circle.SetBool("TurnOn", false);
            ;
        }
    }

    #region Transition
    private void turnedEngineOff()
    {
        _myCheatImage.SetActive(false);

        transform.DOMove(_statePos[0].position, 0.2f, false);
        transform.DOScale(_baseScale * 0.8f, 0.2f);
        u_Circle.SetBool("TurnOn", false); //delete later
    }
    
    private void prepareEngine()
    {
        _myCheatImage.SetActive(false);

        transform.DOMove(_statePos[1].position, 0.2f, false);
        transform.DOScale(_baseScale, 0.2f);
        u_Circle.SetBool("TurnOn", false); //delete later
    }

    private void turnOnEngine()
    {
        transform.DOMove(_statePos[2].position, 0.2f, false);
        transform.DOScale(_baseScale, 0.2f);
        u_Circle.SetBool("TurnOn", true); //delete later
    }
    
    #endregion

    public void playAuraAnim()
    {
        slotAuraAnim.Play("EngineAura_On", -1 , 0f);
    }

    public void selectGear()
    {
        if (_gearAnim.GetBool("TurnOn") == false) _gearAnim.SetBool("TurnOn", true); //pop desc. window
        //transform.DOScale(_baseScale * 1.2f, 0.2f); //change size
        if (!BattleManager.Instance.isMouseDragging && !blockAura) slotAuraAnim.Play("EngineAura_On", -1 , 0f); //play anim
        u_selectedAura.color = Color.white; //turn on Aura for selected
        blockAura = true;
    }

    public void disselectGear()
    {
        if (_gearAnim.GetBool("TurnOn") == true) _gearAnim.SetBool("TurnOn", false); //pop down desc. window
        //transform.DOScale(_baseScale, 0.2f); //return size
        transform.DOScale(_baseScale, 0.2f); //return size
        u_selectedAura.color = new Color(1,1,1,0); //turn off Aura for selected
    }

    private void updateSlotFilled()
    {
        for (int i = 0; i < u_slotFilledAura.Length; i++)
        {
            if (i < _pending.Count)
            {
                u_slotFilledAura[i].color = Color.white;
            }
            else
            {
                u_slotFilledAura[i].color = new Color(1,1,1,0);
            }
        }
    }

    public void attackOnPositionPreviewOn()
    {
        for (int i = 0; i < u_AttackOnPosition.Length ; i++)
        {
            switch (Mathf.Abs(i - BattleManager.Instance.Player.Position))
            {
                case 0 :
                    if (_attackPower[0] > 0) u_AttackOnPosition[i].GetComponent<Animator>().SetBool("TurnOn", true);
                    u_AttackOnPosNumber[i].text = _attackPower[0].ToString();
                    break;
                case 1 :
                    if (_attackPower[1] > 0) u_AttackOnPosition[i].GetComponent<Animator>().SetBool("TurnOn", true);
                    u_AttackOnPosNumber[i].text = _attackPower[1].ToString();
                    break;
                case 2 :
                    if (_attackPower[2] > 0) u_AttackOnPosition[i].GetComponent<Animator>().SetBool("TurnOn", true);
                    u_AttackOnPosNumber[i].text = _attackPower[2].ToString();
                    break;
                default:
                    break;
            }
        }
    }

    public void attackOnPositionPreviewOff()
    {
        for (int i = 0; i < u_AttackOnPosition.Length; i++)
        {
            if (u_AttackOnPosition[i].GetComponent<Animator>().GetBool("TurnOn")) u_AttackOnPosition[i].GetComponent<Animator>().SetBool("TurnOn", false);
        }
    }

    // UI interaction
    #region card UI interaction Functions

    public void moveCardToEngineSlot(int cardEngineIndex, float duration)
    {
        if (cardEngineIndex >= _pending.Count || cardEngineIndex >= _cardPositons.Length)
            return;

            Card thisCard = _pending[cardEngineIndex];
        Transform cardEngineSlot = _cardPositons[0].parent.transform;
        
        thisCard.turnCheatImageRaycast(false); //start with turning off the raycast target to prevent interaction with mouse wile its moving back to position
        
        thisCard.MyCol.enabled = false; //turn off the raycast first so it wont do stuff twice if pointer got out die to moving cursor too fast

        thisCard.transform.SetParent(cardEngineSlot); //change parent first

        //check if it is not in the place already, then move
        if (thisCard.Dragging == false && thisCard.transform.localPosition != CurrentCardPos(cardEngineIndex) && thisCard.Engine != null)
        {
            thisCard.transform.DOLocalMove(CurrentCardPos(cardEngineIndex), duration, false).OnComplete(() => DeckManager.Instance.turnOnRaycast()); //turn it back on after its done
        }
        else
        {
            DeckManager.Instance.turnOnRaycast();
        }
    }

    public void highlightedOn()
    {
        transform.DOScale(_baseScale * 1.2f, 0.2f);
        
        //change card particle size
        if (BattleManager.Instance.BattleState == BattleStates.ChoosingAction)
        {
            foreach (Card C in Stack)
            {
                if (!C.Dragging) C._eventManager.setParticleGlowSize(0.45f);
            }
        }
        else
        {
            for (int i = 0; i < _pending.Count; i++)
            {
                if (_pending[i] != null && !_pending[i].Dragging) _pending[i]._eventManager.setParticleGlowSize(0.45f);
            }   
        }

        _highlighted = true;
    }

    public void highlightedOff()
    {
        transform.DOScale(_baseScale, 0.2f);
        
        //change card particle size
        if (BattleManager.Instance.BattleState == BattleStates.ChoosingAction)
        {
            foreach (Card C in Stack)
            {
                if (!C.Dragging) C._eventManager.setParticleGlowSize(0.37f);
            }
        }
        else
        {
            for (int i = 0; i < _pending.Count; i++)
            {
                if (_pending[i] != null && !_pending[i].Dragging) _pending[i]._eventManager.setParticleGlowSize(0.37f);
            }   
        }

        _highlighted = false;
    }

    public void turnMyCheatImageForRayCastCheck(bool turn)
    {
        //if (_myCheatImageForNewRayCastCheck != turn) 
            _myCheatImageForNewRayCastCheck.SetActive(turn);
    }

    public void blockEngineAuraOff()
    {
        blockAura = false;
    }
    #endregion

    public void StateChange(int state)
    {
        transform.DOMove(_statePos[state].position, 0.3f);

        float brightness = 0;
        float scale = 1f;

        if (state == 0)
        {
            brightness = 0.8f;
            scale = 0.8f;
        }

        _blackMask.DOFade(brightness, 0.3f);
        transform.DOScale(_originalScale * scale, 0.3f);
    }
}

public enum EngineState
{
    Stacking,
    Stacked
}

