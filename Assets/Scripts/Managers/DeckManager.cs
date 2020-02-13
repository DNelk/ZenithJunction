using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using Random = System.Random;
using DG.Tweening;
using TMPro;
using UnityEngine.UIElements;

public class DeckManager : MonoBehaviour
{
    //Singleton
    public static DeckManager Instance;
    
    //Card Vars
    public List<String> Deck = new List<String>(); //User's library of cards (prototype only)
    private List<Card> _activeCards; //Cards in play
    private List<GameObject> _activeCardObjects; //GameObjects of cards in play
    private Stack<String> _discard; //Discarded Cards
    private Stack<String> _deck; //Runtime version of the deck
    private Stack<String> _trash; //Trashed cards (removed until end of battle)
    
    //UI
    private Transform _cardMachine;
    private Transform _cardPanel;
    private TMP_Text[] _inDeckCount;
    private TMP_Text[] _disCount;
    private TMP_Text[] _trashCount;
    private RectTransform[] _cardPositions;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if(Instance != this)
            Destroy(gameObject);

        Init();
    }

    //Initialize Variables
    private void Init()
    {
        _activeCards = new List<Card>();
        _activeCardObjects = new List<GameObject>();
        _discard = new Stack<String>();
        _deck = new Stack<String>();
        _trash = new Stack<String>();

        PlayerCollection pc = Utils.Load<PlayerCollection>("playercollection");
        foreach (String e in pc.Equipped)
        {
            _deck.Push(e);
        }

        _cardPanel = transform.parent;
        _cardMachine = transform.parent.transform.parent;
        
        _inDeckCount = _cardMachine.transform.Find("DeckMachine").transform.Find("Number").GetComponentsInChildren<TMP_Text>();
        _disCount = _cardMachine.transform.Find("DiscardMachine").transform.Find("Number").GetComponentsInChildren<TMP_Text>();    
        _trashCount = _cardMachine.transform.Find("TrashMachine").transform.Find("Number").GetComponentsInChildren<TMP_Text>();

        var positions = transform.Find("CardPositions").GetComponentsInChildren<RectTransform>();
        _cardPositions = new RectTransform[positions.Length];
        int j = 0;
        for (int i = positions.Length - 1; i >= 0; i--)
        {
            _cardPositions[i] = positions[j];
            j++;
        }

    }


    private void Start()
    {
        ShuffleDeck();
        StartCoroutine(DealActive());
    }

    //Shuffle discard into deck
    private void ShuffleDeck()
    {
        if (_discard.Count > 0)
        {
            //Add discard to deck
            String[] discardArr = new String[_discard.Count];
            _discard.CopyTo(discardArr, 0);

            foreach (String c in discardArr)
            {
                _deck.Push(c);
            }
        }
        _discard.Clear();
        //Shuffle
        String[] deckArr = new String[_deck.Count];
        _deck.CopyTo(deckArr, 0);
        //Shuffle up
        deckArr = Utils.Shuffle(deckArr);
        _deck.Clear();
        foreach (String c in deckArr)
        {
            _deck.Push(c);
        }
    }
    
    //Deal 9 to player
    private IEnumerator DealActive()
    {
        for (int i = 0; i < 9; i++)
        {
            if(_deck.Count == 0)
                ShuffleDeck();
            
            GameObject activeCardGO = Instantiate(Resources.Load<GameObject>("Prefabs/Cards/" + _deck.Pop().Replace(" ", String.Empty)), transform.position, Quaternion.identity,
                _cardPanel);
            _activeCardObjects.Add(activeCardGO);

            Card activeCard = activeCardGO.GetComponent<Card>();
            _activeCards.Add(activeCard);
            
            

            Tween dealTween = activeCardGO.transform.DOMove(_cardPositions[i].position, 0.1f, false);
            activeCardGO.transform.localScale = _cardPositions[i].localScale*0.95f;
            yield return dealTween.WaitForCompletion();
        }
    }
    /*
    public void Discard(Engine discardedEngine)
    {
        Stack<Card> toDiscard = discardedEngine.Stack;
        while(toDiscard.Count > 0)
        {
            Card discarding = toDiscard.Pop();
            _discard.Push(discarding.CardName);
            _activeCardObjects.Remove(discarding.gameObject);
            Destroy(discarding.gameObject);
        }
    }
*/
    public void Discard(Card c)
    {
        _discard.Push(c.CardName);
        //Deck.Add(c.CardName);
        if(_activeCardObjects.Contains(c.gameObject))
            _activeCardObjects.Remove(c.gameObject);
        Destroy(c.gameObject);
    }

    public void Discard(string c)
    {
        _discard.Push(c);
    }

    public void Trash(Card c)
    {
        _trash.Push(c.CardName);
        if(_activeCardObjects.Contains(c.gameObject))
            _activeCardObjects.Remove(c.gameObject);
        Destroy(c.gameObject);
    }

    public int InDeckCount() { return _deck.Count; }
    public int InDiscardCount() { return _discard.Count; }
    public int InTrashCount() { return _trash.Count; }
    
    private void Update()
    {
        //update indeck number
        if (InDeckCount() < 10)
        {
            _inDeckCount[0].text = "0";
            _inDeckCount[1].text = InDeckCount().ToString();
        }
        else
        {
            String count = InDeckCount().ToString();
            _inDeckCount[0].text = count[0].ToString();
            _inDeckCount[1].text = count[1].ToString();
        }
        
        //update discard number
        if (InDiscardCount() < 10)
        {
            _disCount[0].text = "0";
            _disCount[1].text = InDiscardCount().ToString();
        }
        else
        {
            String count = InDiscardCount().ToString();
            _disCount[0].text = count[0].ToString();
            _disCount[1].text = count[1].ToString();
        }
        
        //update trash number
        if (InTrashCount() < 10)
        {
            _trashCount[0].text = "0";
            _trashCount[1].text = InTrashCount().ToString();
        }
        else
        {
            String count = InTrashCount().ToString();
            _trashCount[0].text = count[0].ToString();
            _trashCount[1].text = count[1].ToString();
        }
        
        /*_counts[0].text = InDeckCount().ToString();
        _counts[1].text = InDiscardCount().ToString();
        _counts[2].text = InTrashCount().ToString();*/

    }

    public void Reset()
    {
        StartCoroutine(DealActive());
    }
}
