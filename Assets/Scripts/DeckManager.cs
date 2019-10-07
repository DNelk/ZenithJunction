using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using Random = System.Random;
using DG.Tweening;
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
    
    
    //UI
    private Transform _cardPanel;
    public float XInterval;

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
        foreach (String e in Deck)
        {
            _deck.Push(e);
        }

        _cardPanel = transform.parent;
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
        
        //Shuffle
        Random rng = new Random();
        //Convert to array
        String[] deckArr = new String[_deck.Count];
        _deck.CopyTo(deckArr, 0);
        //Shuffle up
        int i = deckArr.Length;
        while (i > 1)
        {
            i--;
            int k = rng.Next(i + 1);
            String tempC = deckArr[k];
            deckArr[k] = deckArr[i];
            deckArr[i] = tempC;
        }
        
        _deck.Clear();
        foreach (String c in deckArr)
        {
            _deck.Push(c);
        }
    }
    
    //Deal 9 to player
    private IEnumerator DealActive()
    {
        float x = transform.position.x;
        for (int i = 0; i < 9; i++)
        {
            if(_deck.Count == 0)
                ShuffleDeck();

            x -= XInterval * (Screen.currentResolution.width/800f);

            GameObject activeCardGO = Instantiate(Resources.Load<GameObject>("Prefabs/Cards/" + _deck.Pop().Replace(" ", String.Empty)), transform.position, Quaternion.identity,
                _cardPanel);
            _activeCardObjects.Add(activeCardGO);

            Card activeCard = activeCardGO.GetComponent<Card>();
            _activeCards.Add(activeCard);
            
            

            Tween dealTween = activeCardGO.transform.DOMoveX(x, 0.2f, false);

            yield return dealTween.WaitForCompletion();
        }
    }
    
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

    public void Discard(Card c)
    {
        _discard.Push(c.CardName);
        Deck.Add(c.CardName);
        if(_activeCardObjects.Contains(c.gameObject))
            _activeCardObjects.Remove(c.gameObject);
        Destroy(c.gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && _activeCards.Count > 0)
        {
            
        }
    }

    public void Reset()
    {
        StartCoroutine(DealActive());
    }
}
