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
    }

    //Adds a card to the pending card array
    public void AddCard(Card c)
    {
        if(c.Manager == this)
            return;
        
        if (c.Manager != null)
            c.Manager.RemoveCard(c);
        
        c.Manager = this;
        c.SetEngine(GlowColor, transform);
        _pending.Add(c);
    }

    public void RemoveCard(Card c)
    {
        _pending.Remove(c);
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
        currentCard.transform.SetSiblingIndex(indexToStack);
        Vector3 targetPos = transform.position + (Vector3.right * Stack.Count * DeckManager.Instance.XInterval);
        currentCard.transform.DOMove(targetPos, 1f, false);
        //currentCard.transform.position = targetPos;
        Stack.Push(currentCard);
        _pending.RemoveAt(indexToStack);
        
        if(_pending.Count > 0)
            StackCards();
    }

    public IEnumerator ExecuteStack()
    {
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
            else if(c.PayForCard())
                c.Execute();
            else
            {
                Debug.Log("not enough steam");
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
        if(c.Manager != null && c.Manager.EngineState == EngineState.Stacked || c.Purchasable)
            return;
        AddCard(c); 
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
            transform.GetComponent<Image>().enabled = false;
            transform.GetComponentInChildren<Text>().enabled = false;
            transform.GetComponent<BoxCollider2D>().offset = new Vector2(44.7f, -0.51f);
            transform.GetComponent<BoxCollider2D>().size = new Vector2(153.2f, 198.7f);
            foreach (Card c in Stack)
            {
                c.GetComponent<Collider2D>().enabled = false;
            }
        }
        else
        {
            EngineState = EngineState.Stacking;
            transform.GetComponent<Image>().enabled = true;
            transform.GetComponentInChildren<Text>().enabled = true;
            transform.GetComponent<BoxCollider2D>().offset = new Vector2(0.2134628f, 0.6905212f);
            transform.GetComponent<BoxCollider2D>().size = new Vector2(99.61905f, 98.34377f);
            transform.DOMove(_initialPosition, 0.5f);
            Executed = false;
        }
    }
    
    #region Card Dragging
    
    //Drag Stuff
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

    
    public void Deselect()
    {
        foreach (Card c in Stack)
        {
            c.SetEngine(GlowColor, transform);
        }
    }
    #endregion
}

public enum EngineState
{
    Stacking,
    Stacked
}
