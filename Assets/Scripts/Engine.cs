using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
    private GameObject u_Circle;
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
    
    //animator
    private Animator slotAuraAnim;

    //for state change event
    [HideInInspector] public Vector3 _baseScale;
    private GameObject _myCheatImage;
    [Range(1, 3)] public int engineNumber = 1;
    [HideInInspector] public List<Transform> _statePos;

    [SerializeField] private List<Card> _pending;
    private Vector3 _initialPosition;
    private BoxCollider2D _collider;

    private bool _selected;
    
    //for pointer event
    private GameObject _gear;
    private Animator _gearAnim;

    //Game Vars
    //Total attack this round
    private int _powerTotal;
    public int PowerTotal
    {
        get => _powerTotal;
        set => _powerTotal = value;
    }
    
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
        
        u_Circle = transform.Find("MagicCircle").gameObject;
        u_CircleGlowMat = Instantiate(u_Circle.GetComponent<Image>().material);
        u_Circle.GetComponent<Image>().material = u_CircleGlowMat;
        u_selectedAura = transform.Find("SelectedAura").GetComponent<Image>();

        //u_EngineImg = transform.Find("EngineImg").gameObject;
        //u_EngineImgAnim = u_EngineImg.GetComponent<Animator>();

        _tooltip = null;
        
        _cardPositons = transform.Find("CardPositions").GetComponentsInChildren<RectTransform>();
        
        //set up the position to go in each state
        Transform[] _stateTran= transform.parent.transform.Find("EnginePos" + engineNumber).GetComponentsInChildren<Transform>();
        Transform[] temp = new Transform[3];
        for (int i = 1; i < _stateTran.Length; i++)
        {
            temp[i - 1] = _stateTran[i];
        }
        _stateTran = temp;
        foreach (var trans in _stateTran)
        {
            _statePos.Add(trans);
        }

        //set up images and cheatImg for state change
        _baseScale = transform.localScale;
        _myCheatImage = transform.Find("CheatImg").gameObject;
        
        //anim
        slotAuraAnim = transform.Find("SlotAura").GetComponent<Animator>();
        _gear = transform.Find("Gear").gameObject;
        _gearAnim = _gear.GetComponent<Animator>();
        //set Gear Sprite 
        Image engineNumGear = _gearAnim.transform.Find("EngineNumber").GetComponent<Image>();
        engineNumGear.sprite = Resources.Load<Sprite>("Sprites/Engine" + engineNumber);

        //set up number and Icon for total engine power
        u_powerNumber = transform.Find("PowerNumber").GetComponent<TMP_Text>();
        u_powerCore = transform.Find("AttackEngine").transform.Find("Core_Main").GetComponent<Image>();
        u_aetherNumber = transform.Find("AetherNumber").GetComponent<TMP_Text>();
        u_aetherCore = transform.Find("AetherEngine").transform.Find("Core_Main").GetComponent<Image>();
        u_move = transform.Find("MoveIcon").GetComponentsInChildren<Image>();
        
        OverridePower = OverrideAether = OverrideMove = -1;
    }

    //Adds a card to the pending card array
    public void AddCard(Card c)
    {
        if(c.Engine == this)
            return;
        
        if(PendingCount >= 3)
            return;
        
        if (c.Engine != null)
            c.Engine.RemoveCard(c, false);

        c.Engine = this;
        c.SetEngine(_cardPositons[0].parent.transform, CurrentCardPos(_pending.Count), 0.7f);
        _pending.Add(c);
        DeckManager.Instance.CardsToBeSorted.Remove(c);
        UpdateUICounts();
        
        //turn on the circle
        MagicCircle();
    }

    public void RemoveCard(Card c, bool isClick)
    {
        int cInd = _pending.IndexOf(c);
        Card nextC = null;
        
        if(_pending.Count > 1 && cInd != _pending.Count - 1)
            nextC = _pending[cInd + 1];
        
        _pending.Remove(c);
        DeckManager.Instance.CardsToBeSorted.Add(c);
        
        UpdateUICounts();
        c.OffEngine(DeckManager.Instance.transform.parent, 1/0.7f);
        c.Engine = null;
        if (isClick)
        {
            DeckManager.Instance.moveCardsToTray(c.MyIndex,0.5f);
        }
        
        //turn off magic circle
        MagicCircle();
        
        if(cInd == _pending.Count)
            return;
        if(_pending.Count == 0)
            return;
        nextC.transform.position = CurrentCardPos(cInd);
        while (cInd < _pending.Count-1)
        {
            cInd++;
            nextC = _pending[cInd];
            nextC.transform.DOMove(CurrentCardPos(cInd), 0.1f);
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
        currentCard.transform.position = CurrentCardPos(Stack.Count);
        Stack.Push(currentCard);
        _pending.RemoveAt(indexToStack);
        
        if(_pending.Count > 0)
            StackCards();
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

        Card currentCard = _pending[indexToStack];
        Stack.Push(currentCard);
        _pending.RemoveAt(indexToStack);
        
        if(_pending.Count > 0)
            StackCardsForPreview();
    }

    public List<Card> PoppedCards; //Cards go here between execution steps 1 and 2
    public bool GoldOrSilverFound = false;  //Boolean for the Golden/Silver combo cards
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
        Player player = BattleManager.Instance.Player;
        if (player.ActiveStats.ContainsKey(StatType.MovesUP))
        {
            if(!player.ActiveStats[StatType.MovesUP].IsNew)
                _moveTotal += player.ActiveStats[StatType.MovesUP].Value;
        }
        if (player.ActiveStats.ContainsKey(StatType.MovesDOWN))
        {
            if(!player.ActiveStats[StatType.MovesDOWN].IsNew)
                _moveTotal -= player.ActiveStats[StatType.MovesDOWN].Value;
        }
        
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
        
        MagicCircle();
    }
    private int ExecuteStackForPreview()
    { 
        int totalCost = 0;

        List<Card> tempPending = new List<Card>(_pending);
        
        if (EngineState == EngineState.Stacking)
            StackCardsForPreview();
        
        

        foreach (var c in Stack)
        {
            if (c.IsXCost) //-1 is X
            {
                continue;
            }

            c.Execute();

            _powerTotal += c.CalculateAttackTotalWithPosition();
            _aetherTotal += c.AetherTotal;
            _moveTotal += c.MoveTotal;
            totalCost += c.AetherCost;
            if (!c.IsAttackInRange() && _inRange)
                _inRange = false;
        }

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
        if(!_selected)
            u_CircleGlowMat.SetColor("_MyColor", GlowColor);
        if(_tooltip == null)
            return;
        _tooltip.StartCoroutine(_tooltip.FadeOut());
        _tooltip = null;
    }
    
    //Collisions
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Card") || EngineState != EngineState.Stacking)
            return;
        Card c = other.gameObject.GetComponent<Card>();
        if(c.Engine != null  && c.Engine.EngineState == EngineState.Stacked|| c.Purchasable || c.Dragging || c.Tweening || c.IsPreview)
            return;
        AddCard(c);
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        OnTriggerEnter2D(other);
    }

   /* private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Card"))
            return;
        Card c = other.gameObject.GetComponent<Card>();
        RemoveCard(c);
        c.SetEngine(Color.clear, transform.parent);
    }*/

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
            
            u_Circle.GetComponent<Image>().enabled = true;
        }
        else if (PendingCount < 3 && _wheelTurning && EngineState != EngineState.Stacked || (EngineState == EngineState.Stacked && Stack.Count == 0))
        {
           // u_EngineImgAnim.SetBool("IsReady", false);
            //_steamParticle.Stop();
            _wheelTurning = false;
            u_Circle.GetComponent<Image>().enabled = false;
        }
        
        if (Input.GetKeyDown(KeyCode.A)) turnedEngineOff();
        if (Input.GetKeyDown(KeyCode.S)) prepareEngine();
        if (Input.GetKeyDown(KeyCode.D)) turnOnEngine();
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
        
        if(setToZero)
            tempPow = tempAet = tempMove = tempCost = 0;
        
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
        u_CircleGlowMat.SetColor("_MyColor", GlowColor);
        _selected = false;
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
        return _cardPositons[count].position;
    }

    private void MagicCircle()
    {
        if(PendingCount >= 3) u_Circle.SetActive(true);
        else
        {
            u_Circle.SetActive(false);
        }
    }

    private void turnedEngineOff()
    {
        _myCheatImage.SetActive(false);

        transform.DOMove(_statePos[0].position, 0.2f, false);
        transform.DOScale(_baseScale * 0.8f, 0.2f);
        u_Circle.SetActive(false); //delete later
    }
    
    private void prepareEngine()
    {
        _myCheatImage.SetActive(false);

        transform.DOMove(_statePos[1].position, 0.2f, false);
        transform.DOScale(_baseScale, 0.2f);
        u_Circle.SetActive(false); //delete later
    }

    private void turnOnEngine()
    {
        transform.DOMove(_statePos[2].position, 0.2f, false);
        transform.DOScale(_baseScale, 0.2f);
        u_Circle.SetActive(true); //delete later
    }

    public void playAuraAnim()
    {
        slotAuraAnim.Play("EngineAura_On", -1 , 0f);
    }

    public void selectGear()
    {
        if (_gearAnim.GetBool("TurnOn") == false) _gearAnim.SetBool("TurnOn", true); //pop desc. window
        //transform.DOScale(_baseScale * 1.2f, 0.2f); //change size
        slotAuraAnim.Play("EngineAura_On", -1 , 0f); //play anim
        u_selectedAura.color = Color.white; //turn on Aura for selected
    }

    public void disselectGear()
    {
        if (_gearAnim.GetBool("TurnOn") == true) _gearAnim.SetBool("TurnOn", false); //pop down desc. window
        //transform.DOScale(_baseScale, 0.2f); //return size
        transform.DOScale(_baseScale, 0.2f); //return size
        u_selectedAura.color = new Color(1,1,1,0); //turn off Aura for selected
    }
}

public enum EngineState
{
    Stacking,
    Stacked
}
