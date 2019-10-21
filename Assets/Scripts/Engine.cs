using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class Engine : MonoBehaviour
{
    //Instance
    //public static Engine Instance = null;

    public Stack<Card> Stack;
    
    //UI
    public Color GlowColor;
    private GameObject[] u_Circles;
    private GameObject[] u_Wheels;
    private GameObject u_EngineImg;
    private GameObject u_HolesImg;
    private Material[] u_WheelMat;
    private bool _wheelTurning;

    [SerializeField] private List<Card> _pending;
    private Vector3 _initialPosition;
    private BoxCollider2D _collider;


    //Total attack this round
    private int _atkTotal;
    public int AtkTotal
    {
        get => _atkTotal;
        set => _atkTotal = value;
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
    
    public EngineState EngineState;

    public float XInterval = 50f;

    public bool Executed;
    private void Awake()
    {
       /*//Check if Instance already exists
        if (Instance == null)             
            //if not, set Instance to this
            Instance = this;
            
        //If Instance already exists and it's not this:
        else if (Instance != this)   
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one Instance of a GameManager.
            Destroy(gameObject);
    */
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
        
        u_Circles = new GameObject[2];
        u_Circles[0] = transform.Find("CircleGlow").gameObject;
        u_Circles[1] = u_Circles[0].transform.Find("CircleFlat").gameObject;
        Material mat = Instantiate(u_Circles[0].GetComponent<Image>().material);
        mat.SetColor("_MyColor", GlowColor);
        //u_Circles[0].GetComponent<Image>().material.SetColor("_MyColor", GlowColor);
        u_Circles[0].GetComponent<Image>().material = mat;
        
        u_Wheels = new GameObject[2];
        u_Wheels[0] = u_Circles[0].transform.Find("WheelGlow").gameObject;
        u_Wheels[1] = u_Wheels[0].transform.Find("Wheel").gameObject;
        u_WheelMat = new Material[2];
        u_WheelMat[0] = Instantiate(u_Wheels[0].GetComponent<Image>().material);
        u_WheelMat[0].SetColor("_MyColor", GlowColor);
        u_Wheels[0].GetComponent<Image>().material = u_WheelMat[0];
        u_WheelMat[1] = Instantiate(u_Wheels[1].GetComponent<Image>().material);
        u_Wheels[1].GetComponent<Image>().material = u_WheelMat[1];
        _wheelTurning = false;

        u_EngineImg = u_Circles[0].transform.Find("EngineImg").gameObject;
        u_HolesImg = u_EngineImg.transform.Find("EngineHoles").gameObject;
    }

    //Adds a card to the pending card array
    public void AddCard(Card c)
    {
        if(c.Engine == this)
            return;
        
        if(PendingCount == 3)
            return;
        
        if (c.Engine != null)
            c.Engine.RemoveCard(c);

        c.Engine = this;
        c.SetEngine(GlowColor, u_Circles[1].transform, CurrentCardPos(_pending.Count));
        _pending.Add(c);
    }

    public void RemoveCard(Card c)
    {
        int cInd = _pending.IndexOf(c);
        Card nextC = null;
        
        if(_pending.Count > 1 && cInd != _pending.Count - 1)
            nextC = _pending[cInd + 1];
        
        _pending.Remove(c);
        
        if(cInd == _pending.Count)
            return;
        if(_pending.Count == 0)
            return;
        nextC.transform.position = CurrentCardPos(cInd);
        while (cInd < _pending.Count-1)
        {
            cInd++;
            nextC = _pending[cInd];
            nextC.transform.position = CurrentCardPos(cInd);
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
        
        currentCard.ReadyCard();
        currentCard.transform.SetSiblingIndex(Stack.Count);
        currentCard.transform.position = CurrentCardPos(Stack.Count);
        Stack.Push(currentCard);
        _pending.RemoveAt(indexToStack);
        
        if(_pending.Count > 0)
            StackCards();
    }

    public IEnumerator ExecuteStack()
    {
        BattleDelegateHandler.ApplyEngineEffects();
        while (Stack.Count > 0)
        {
            Card c = Stack.Pop();
            if (c.IsXCost) //-1 is X
            {
                XDialog xd = Instantiate(Resources.Load<GameObject>("prefabs/xdialog"), GameObject.Find("Canvas").transform).GetComponent<XDialog>();
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
            _atkTotal += c.CalculateAttackTotalWithPosition();
            _aetherTotal += c.AetherTotal;
            _moveTotal += c.MoveTotal;
            if (c.TrashThis)
                DeckManager.Instance.Trash(c);
            else
                DeckManager.Instance.Discard(c);
        }

        Executed = true;
    }
    
    //Collisions
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Card") || EngineState != EngineState.Stacking)
            return;
        Card c = other.gameObject.GetComponent<Card>();
        if(c.Engine != null && c.Engine.EngineState == EngineState.Stacked || c.Purchasable || c.Dragging || c.Tweening || c.IsPreview)
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
        if (PendingCount >= 3 || EngineState == EngineState.Stacked && !_wheelTurning)
        {
            for (int i = 0; i < u_WheelMat.Length; i++)
            {
                u_WheelMat[i].SetFloat("_TimeScale", 100f);
                u_Wheels[i].GetComponent<Image>().material = u_WheelMat[i];
            }

            _wheelTurning = true;
        }
        else if (PendingCount < 3 && _wheelTurning && EngineState != EngineState.Stacked)
        {
            for (int i = 0; i < u_WheelMat.Length; i++)
            {
                u_WheelMat[i].SetFloat("_TimeScale", 0);
                u_Wheels[i].GetComponent<Image>().material = u_WheelMat[i];
            }

            _wheelTurning = false;
        }
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
            GetComponentInChildren<Text>().enabled = false;
        }
        else
        {
            EngineState = EngineState.Stacking;
            Executed = false;
            GetComponentInChildren<Text>().enabled = true;
            _aetherTotal = 0;
            _atkTotal = 0;
            _moveTotal = 0;
        }
    }
    
    #region Card Dragging
    
    /*//Drag Stuff
    private Vector3 _screenPoint;
    private Vector3 _offset;

    public void OnMouseDown()
    {
        if(EngineState != EngineState.Stacked)
            return;
        CalcOffset();
        if(BattleManager.Instance.PlayerAttack == this || Stack.Count == 0)
            return;
        foreach (Card c in Stack)
        {
            c.SetEngine(Color.white, transform);
        }

        BattleManager.Instance.PushAttack(this);
    }


    void OnMouseDrag()
    {
        CalcPosOnMouseMove();
    }
    
    private void CalcOffset()
    {
        _screenPoint = Camera.main.WorldToScreenPoint(transform.position);


        _offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));
        //_offset = transform.position - new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
    }

    private void CalcPosOnMouseMove()
    {
        if(EngineState != EngineState.Stacked)
            return;
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y,transform.position.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + _offset;
        transform.position = curPosition;
    }

    */
    public void Deselect()
    {
        foreach (Card c in Stack)
        {
            c.SetEngine(GlowColor, u_Circles[1].transform);
        }
    }

    public void Select()
    {
        foreach (Card c in Stack)
        {
            c.SetEngine(Color.white, u_Circles[1].transform);
        }

        BattleManager.Instance.PushAttack(this);
    }
    #endregion

    private Vector3 CurrentCardPos(int count)
    {
        Rect engineRect = u_EngineImg.GetComponent<RectTransform>().rect;
        Vector3 enginePos = u_EngineImg.transform.position;
        Vector3 posVec = new Vector3(enginePos.x + (count * (XInterval * (Screen.currentResolution.width/800f)) -engineRect.width/10), enginePos.y + engineRect.height/10, 0f);
        return posVec;
    }
}

public enum EngineState
{
    Stacking,
    Stacked
}
