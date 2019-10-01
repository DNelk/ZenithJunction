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

public class EngineManager : MonoBehaviour
{
    //Instance
    //public static EngineManager Instance = null;

    public Stack<Card> Stack;
    public Color GlowColor;
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
    
    //Total steam this round
    private int _steamTotal;
    public int SteamTotal
    {
        get => _steamTotal;
        set => _steamTotal = value;
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
    }

    //Adds a card to the pending card array
    public void AddCard(Card c)
    {
        if(c.Manager == this)
            return;
        
        if (c.Manager != null)
            c.Manager.RemoveCard(c);
        
        c.Manager = this;
        c.SetEngine(GlowColor, transform, CurrentCardPos(_pending.Count));
        _pending.Add(c);
    }

    public void RemoveCard(Card c)
    {
        _pending.Remove(c);
        if(_pending.Count == 0)
            return;
        Card lastC = _pending[_pending.Count - 1];
        lastC.SetEngine(GlowColor, transform, CurrentCardPos(_pending.Count-1));
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
        EngineDelegateHandler.ApplyEngineEffects();
        while (Stack.Count > 0)
        {
            Card c = Stack.Pop();
            if (c.IsXCost) //-1 is X
            {
                XDialog xd = Instantiate(Resources.Load<GameObject>("prefabs/xdialog"), GameObject.Find("Canvas").transform).GetComponent<XDialog>();
                xd.SteamMax = _steamTotal;
                //Wait until user assigns x value
                yield return new WaitUntil(() => xd.XConfirmed);
                _steamTotal -= xd.XValue;
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
                Utils.DisplayError("Not enough steam to power " + c.CardName + "!", 0.5f);
                c.ExecuteFailed();
            }
            _atkTotal += c.AtkMod;
            _steamTotal += c.SteamMod;
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
        if(c.Manager != null && c.Manager.EngineState == EngineState.Stacked || c.Purchasable || c.Dragging)
            return;
        AddCard(c); 
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        OnTriggerEnter2D(other);
    }

    public int Count
    {
        get => _pending.Count;
    }
    
    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Return) && _pending.Count > 0)
        {
            StackCards();
            ExecuteStack();
            Debug.Log("Total Attack: " + _atkTotal);
            Debug.Log("Total Focus: " + _focTotal);

        }*/
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
            _steamTotal = 0;
            _atkTotal = 0;
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
            c.SetEngine(GlowColor, transform);
        }
    }

    public void Select()
    {
        foreach (Card c in Stack)
        {
            c.SetEngine(Color.white, transform);
        }

        BattleManager.Instance.PushAttack(this);
    }
    #endregion

    private Vector3 CurrentCardPos(int count)
    {
        return (transform.position + (Vector3.left*_collider.size.x/2)) + (Vector3.right * (count * (XInterval * (Screen.currentResolution.width/800f))));
    }
}

public enum EngineState
{
    Stacking,
    Stacked
}
