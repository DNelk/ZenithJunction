using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

    private ParticleSystem _steamParticle;
    
    [SerializeField] private List<Card> _pending;
    private Vector3 _initialPosition;
    private BoxCollider2D _collider;

    private bool _selected;

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

    private bool _inRange = true;
    
    public EngineState EngineState;
    
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

        //u_EngineImg = transform.Find("EngineImg").gameObject;
        //u_EngineImgAnim = u_EngineImg.GetComponent<Animator>();

        _steamParticle = transform.Find("Steam").GetComponent<ParticleSystem>();
        
        _tooltip = null;
        
        _cardPositons = transform.Find("CardPositions").GetComponentsInChildren<RectTransform>();
        
        //set up number and Icon for total engine power
        u_powerNumber = transform.Find("PowerNumber").GetComponent<TMP_Text>();
        u_powerCore = transform.Find("AttackEngine").transform.Find("Core_Main").GetComponent<Image>();
        u_aetherNumber = transform.Find("AetherNumber").GetComponent<TMP_Text>();
        u_aetherCore = transform.Find("AetherEngine").transform.Find("Core_Main").GetComponent<Image>();
        u_move = transform.Find("MoveIcon").GetComponentsInChildren<Image>();
    }

    //Adds a card to the pending card array
    public void AddCard(Card c)
    {
        if(c.Engine == this)
            return;
        
        if(PendingCount >= 3)
            return;
        
        if (c.Engine != null)
            c.Engine.RemoveCard(c);

        c.Engine = this;
        c.SetEngine(_cardPositons[0].parent.transform, CurrentCardPos(_pending.Count), _cardPositons[0].parent.localScale *0.8f);
//        Debug.Log(transform.localScale.x);
        _pending.Add(c);
        UpdateUICounts();
    }

    public void RemoveCard(Card c)
    {
        int cInd = _pending.IndexOf(c);
        Card nextC = null;
        
        if(_pending.Count > 1 && cInd != _pending.Count - 1)
            nextC = _pending[cInd + 1];
        
        _pending.Remove(c);
        
        UpdateUICounts();
        
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

    private List<Card> _poppedCards;
    public IEnumerator ExecuteStack()
    {
        BattleDelegateHandler.ApplyEngineEffects();
        ReadyCards();
        _poppedCards = new List<Card>();
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
            _poppedCards.Add(c);
        }
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
        Deselect();
    }

    //Last stage of execution
    public void CalculatePowerTotal()
    {
        foreach (var c in _poppedCards)
        {
            _powerTotal += c.CalculateAttackTotalWithPosition();
            //Finish Executing
            if (c.TrashThis)
                DeckManager.Instance.Trash(c);
            else
                DeckManager.Instance.Discard(c);
        }
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

            _powerTotal += c.PowerTotal;
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
        u_CircleGlowMat.SetColor("_MyColor", Color.yellow);
        
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
        
        _tooltip = Instantiate(Resources.Load<GameObject>("prefabs/tooltip"), transform.GetChild(0).transform.position,
            Quaternion.identity, GameObject.Find("MainCanvas").transform).GetComponent<Tooltip>();
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
            if(_steamParticle.isPlaying)
                _steamParticle.Stop();
            _steamParticle.Play();
            _wheelTurning = true;
            
            u_Circle.GetComponent<Image>().enabled = true;
        }
        else if (PendingCount < 3 && _wheelTurning && EngineState != EngineState.Stacked || (EngineState == EngineState.Stacked && Stack.Count == 0))
        {
           // u_EngineImgAnim.SetBool("IsReady", false);
            _steamParticle.Stop();
            _wheelTurning = false;
            u_Circle.GetComponent<Image>().enabled = false;
        }
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
            u_powerCore.sprite = Resources.Load<Sprite>("Sprites/Core/AttackCore_On");
            u_powerCore.SetNativeSize();
        }
        else
        {
            u_powerCore.sprite = Resources.Load<Sprite>("Sprites/Core/AttackCore_Off");
            u_powerCore.SetNativeSize();
        }

        if (tempAet > 0)
        {
            u_aetherCore.sprite = Resources.Load<Sprite>("Sprites/Core/ManaCore_On");
            u_aetherCore.SetNativeSize();
        }
        else
        { 
            u_aetherCore.sprite = Resources.Load<Sprite>("Sprites/Core/ManaCore_Off");
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
            _inRange = true;
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
        if(EngineState != EngineState.Stacked || BattleManager.Instance.BattleState != BattleStates.ChoosingAction || Stack.Count < 3)
            return;
        u_CircleGlowMat.SetColor("_MyColor", Color.yellow);
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
}

public enum EngineState
{
    Stacking,
    Stacked
}
