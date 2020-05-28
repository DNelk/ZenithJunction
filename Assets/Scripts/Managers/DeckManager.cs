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
    public List<Card> CardsToBeSorted;
    
    //UI
    private Transform _cardMachine;
    private Animator[] _cardTabLock;
    private ParticleSystem[] _tabParticles;
    private Transform _cardPanel;
    private TMP_Text[] _inDeckCount;
    private TMP_Text[] _disCount;
    private TMP_Text[] _trashCount;
    [HideInInspector] public RectTransform[] _cardPositions;
    
    //Other Vars
    public int DealAmt;
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
        CardsToBeSorted = new List<Card>();
        
        PlayerCollection pc = Utils.Load<PlayerCollection>("playercollection");
        foreach (String e in pc.Equipped)
        {
            _deck.Push(e);
        }

        _cardPanel = transform.parent;
        _cardMachine = transform.parent.transform.parent;
        _cardTabLock = _cardMachine.transform.Find("CardTab").transform.Find("TabLocks").GetComponentsInChildren<Animator>();
        _tabParticles = _cardMachine.transform.Find("CardTab").transform.Find("TabParticle").GetComponentsInChildren<ParticleSystem>();
        ;

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

        DealAmt = 9;
    }


    private void Start()
    {
        ShuffleDeck();
    }

    public void DealHand()
    {
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
        //Palmmy
        unlockTab();
        yield return new WaitForSeconds(0.5f);
        
        //Dylan
        int totalCardsNum = _deck.Count + _discard.Count;
        int tempDealAmt = DealAmt;
        if (totalCardsNum < DealAmt)
            DealAmt = totalCardsNum;
        
        for (int i = 0; i < DealAmt; i++)
        {
            if(_deck.Count == 0)
                ShuffleDeck();
            
            GameObject activeCardGO = Instantiate(Utils.LoadCard(_deck.Pop()), transform.position, Quaternion.identity,
                _cardPanel);
            _activeCardObjects.Add(activeCardGO);

            Card activeCard = activeCardGO.GetComponent<Card>();
            CardsToBeSorted.Add(activeCard);
            
            if (_activeCards.Count <= i) _activeCards.Add(activeCard);
            else
            {
                _activeCards[i] = activeCard;
            }

            Tween dealTween = activeCardGO.transform.DOMove(_cardPositions[i].position, 0.1f, false);
            
            //for Palmmy
            activeCard.InActive = true;
            activeCard.MyIndex = i;
            activeCard._inSlot = true;
            //_cardPositions[i].GetComponent<BoxCollider2D>().enabled = true;

            activeCardGO.transform.localScale = _cardPositions[i].localScale*0.975f;
            
        }

        yield return new WaitForSeconds(0.1f);
        
        playUnlockTabParticle();
        turnOnRaycast();

        DealAmt = tempDealAmt;
    }

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

        if (Input.GetKeyDown(KeyCode.R) && BattleManager.Instance.BattleState == BattleStates.MakingEngines)
        {
            StartCoroutine(RandomizeEngines());
        }
    }

    public void Reset()
    {
        StartCoroutine(DealActive());
    }

    public void moveCardsToTray(int cardIndex, float duration)
    {
        _activeCards[cardIndex].MyCol.enabled = false;

        if (_activeCards[cardIndex].Dragging != true && _activeCards[cardIndex].transform.position != _cardPositions[cardIndex].position && _activeCards[cardIndex].Engine == null)
        { 
            _activeCards[cardIndex].transform.DOMove(_cardPositions[cardIndex].position, duration, false);
        }
        
        //make sure that after it move to tray, it declare to be in slot
        if (!_activeCards[cardIndex]._inSlot) _activeCards[cardIndex]._inSlot = true;
    }

    public void swapCardLocation(int currentIndex, int newIndex)
    {
        var temp = _activeCards[currentIndex];
        
        if (newIndex > currentIndex)
        {
            for (int i = currentIndex; i < newIndex; i++)
            {
                _activeCards[i] = _activeCards[i + 1];
                _activeCards[i].MyIndex = i;
                _activeCards[i + 1] = null;
                if (_activeCards[i]._inSlot == true) moveCardsToTray(i, 0.1f);
            }
        }
        else if (newIndex < currentIndex)
        {
            for (int i = currentIndex; i > newIndex; i--)
            {
                _activeCards[i] = _activeCards[i - 1];
                _activeCards[i].MyIndex = i;
                _activeCards[i - 1] = null;
                if (_activeCards[i]._inSlot == true) moveCardsToTray(i, 0.1f);
            }
        }
        
        _activeCards[newIndex] = temp;
    }

    private void unlockTab()
    {
        foreach (var anim in _cardTabLock)
        {
            if (anim.GetBool("isActive") != true) anim.SetBool("isActive", true);
        }
    }

    private void playUnlockTabParticle()
    {
        foreach (var particle in _tabParticles)
        {
            particle.Play();
        }
    }

    public void lockTab()
    {
        foreach (var anim in _cardTabLock)
        {
            if (anim.GetBool("isActive") != false) anim.SetBool("isActive", false);
        }
    }
    
    public void playLockTabParticle()
    {
        foreach (var particle in _tabParticles)
        {
            particle.Stop();
        }
    }

    public void turnOnRaycast()
    {
        foreach (var card in _activeCards)
        {
            if (_activeCards != null) card.MyCheatImg.SetActive(true);
        }
    }

    public void turnOffOtherRaycast(int cardIndex)
    {
        foreach (var card in _activeCards)
        {
            if (_activeCards != null)
            {
                if (card.MyIndex != cardIndex) card.MyCheatImg.SetActive(false);
            }
        }
    }

    public void LoadDeck(List<string> deck)
    {
        ShuffleDeck();
        _deck = new Stack<string>();
        foreach (var c in deck)
        {
            _deck.Push(c);
        }
    }

    private IEnumerator RandomizeEngines()
    {
        foreach (var c in _activeCards)
        {
            if(c.Engine == null) 
                BattleManager.Instance.GetNextOpenEngine().AddCard(c);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
