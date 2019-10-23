using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class BuyManager : MonoBehaviour
{
    //Singleton
    public static BuyManager Instance;

    //UI
    private CanvasGroup _cg;
    public Transform DeckPos;
    public float XInterval;
    private Button _leaveButton;
    private Transform _cardParent;
    private Text _aetherText;
    private Text _buyText;
    private Text _freeBuyText;

    
    //Vars
    private List<GameObject> _activeCardObjects;
    private List<Card> _activeCards;
    [SerializeField] private List<String> _catalog = new List<String>();
    private Stack<String> _shopDeck;
    //private Stack<Card> _pending;
    ///private Stack<Vector3> _pendingPos;
    public int BuysRemaining;
    public int FreeBuysRemaining;
    
    //Freebies
    private Card _endlessAtk;
    private Card _endlessAether;
    private List<GameObject> _soldOutMarkers;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        Init();
    }


    private void Init()
    {
        _endlessAtk = transform.Find("EndlessAtk").GetComponent<Card>();
        _endlessAether = transform.Find("EndlessAether").GetComponent<Card>();
        _activeCards = new List<Card>();
        _activeCardObjects = new List<GameObject>();
        _soldOutMarkers = new List<GameObject>();
        _shopDeck = new Stack<String>();
        ShuffleShopDeck();
        _cg = GetComponent<CanvasGroup>();
        _leaveButton = transform.Find("LeaveShop").GetComponent<Button>();
        _cardParent = transform.Find("Cards").transform;
        _aetherText = transform.Find("AetherText").GetComponent<Text>();
        _buyText = transform.Find("BuyText").GetComponent<Text>();
        _freeBuyText = transform.Find("FreeBuyText").GetComponent<Text>();
        BuysRemaining = -1;
        FreeBuysRemaining = 0;
    }

    //Buy a card we click on
    public void BuyCard(Card c)
    {
        if ((BattleManager.Instance.CurrentAether < c.BuyCost && FreeBuysRemaining == 0) || BuysRemaining == 0)
            return;
        
        if (FreeBuysRemaining != 0)
            FreeBuysRemaining--;
        else 
            BattleManager.Instance.CurrentAether -= c.BuyCost;
        
        if (c == _endlessAtk)
        {
            _soldOutMarkers.Add(Instantiate(Resources.Load<GameObject>("Prefabs/SoldOutCard"), c.transform.position, Quaternion.identity, transform));
            DeckManager.Instance.Discard(Instantiate(Resources.Load<GameObject>("Prefabs/Cards/Strike").GetComponent<Card>()));
        }
        else if (c == _endlessAether)
        {
            _soldOutMarkers.Add(Instantiate(Resources.Load<GameObject>("Prefabs/SoldOutCard"), c.transform.position, Quaternion.identity, transform));
            DeckManager.Instance.Discard(c.CardName);
        }
        else
        {
            c.Purchasable = false;
            _activeCardObjects.Remove(c.gameObject);
            _catalog.Remove(c.CardName);
            StartCoroutine(DealNewCard(c.transform.position.x, c.transform.GetSiblingIndex(), _activeCards.IndexOf(c)));
            _activeCards.Remove(c);
            DeckManager.Instance.Discard(c);
        }

        if (BuysRemaining != -1)
            BuysRemaining--;
    }

    public IEnumerator LoadBuyMenu()
    {
        foreach (GameObject go in _soldOutMarkers)
        {
            Destroy(go);
        }
        _soldOutMarkers.Clear();
        
        //Fade in
        _cg.blocksRaycasts = true;
        Tween fade = _cg.DOFade(1.0f, 1.0f);
        yield return fade.WaitForCompletion();
        _cg.interactable = true;

        //Deal Cards
        _activeCards.Clear();
        float x = DeckPos.position.x;
        for (int i = 0; i < 5; i++)
        {
            x -= XInterval * (Screen.currentResolution.width/800f);

            StartCoroutine(DealNewCard(x,i,i));
        }
        _leaveButton.onClick.AddListener(LeaveShop);
    }

    public void LeaveShop()
    {
        StartCoroutine(LeaveBuyMenu());
    }

    private IEnumerator LeaveBuyMenu()
    {
        _leaveButton.onClick.RemoveListener(LeaveShop);
        //Fade in
        for (int i = _activeCards.Count - 1; i >= 0; i--)
        {
            _shopDeck.Push(_activeCards[i].CardName);
        }
        _activeCards.Clear();
        foreach (GameObject go in _activeCardObjects)
        {
            Destroy(go);
        }
        _activeCardObjects.Clear();
        _cg.interactable = false;
        _cg.blocksRaycasts = false;
        Tween fade = _cg.DOFade(0.0f, 1.0f);
        yield return fade.WaitForCompletion();
        BattleManager.Instance.BattleState = BattleStates.ChoosingAction;
       
        BuysRemaining = -1; //By default you have infinite buys
        FreeBuysRemaining = 0;
    }

    private IEnumerator DealNewCard(float xPosition, int siblingIndex, int activeIndex)
    {
        GameObject activeCardGO = Instantiate(Resources.Load<GameObject>("prefabs/cards/" + _shopDeck.Pop().Replace(" ", String.Empty)), DeckPos.position, Quaternion.identity, _cardParent);
        Card activeCard = activeCardGO.GetComponent<Card>();
        activeCardGO.transform.SetSiblingIndex(siblingIndex);
        activeCard.Purchasable = true;
        _activeCards.Insert(activeIndex, activeCard);
            
        _activeCardObjects.Add(activeCardGO);

        Tween dealTween = activeCardGO.transform.DOMoveX(xPosition, 0.2f, false);

        yield return dealTween.WaitForCompletion();
    }

    private void ShuffleShopDeck()
    {
        //Shuffle
        Random rng = new Random();
        //Convert inventory to array
        String[] deckArr = new String[_catalog.Count];
        _catalog.CopyTo(deckArr, 0);
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
        
        _shopDeck.Clear();
        foreach (String c in deckArr)
        {
            _shopDeck.Push(c);
        }
    }

    private void Update()
    {
        _aetherText.text = "Aether Remaining: " + BattleManager.Instance.CurrentAether;
        
        if (BuysRemaining != -1)
            _buyText.text = "Buys Remaining: " + BuysRemaining;
        else
            _buyText.text = "";

        if (FreeBuysRemaining != 0)
            _freeBuyText.text = "Free buys Remaining: " + FreeBuysRemaining;
        else
            _freeBuyText.text = "";
    }
}
