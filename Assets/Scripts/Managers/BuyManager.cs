using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
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
    private Button _showShopButton;
    private TMP_Text _aetherText;
    private Transform _cardParent;
    private RectTransform[] _positions;
    public bool Previewing;
    private GameObject[] _magicCircle = new GameObject[2];
    private GameObject _coreFilled;
    
    //Vars
    private List<GameObject> _activeCardObjects;
    private List<Card> _activeCards;
    [SerializeField] private List<String> _catalog = new List<String>();
    private Stack<String> _shopDeck;
    private Stack<String> _discarded;
    //private Stack<Card> _pending;
    ///private Stack<Vector3> _pendingPos;
    public int BuysRemaining;
    public int FreeBuysRemaining;
    private BattleStates _lastState;
    public int DealAmt = 5;
    
    //Freebies
    private Card _endlessAtk;
    private Card _endlessAether;
    private List<GameObject> _soldOutMarkers;
    
    //Debug
    public bool OverrideStore = false;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }


    private void Start()
    {
        //StartCoroutine(LoadBuyMenu());
        Init();
    }

    private void Init()
    {
        var bg = transform.Find("BG").transform;
        _endlessAtk = transform.Find("EndlessAtk").GetComponent<Card>();
        _endlessAether = transform.Find("Free").Find("EndlessAether").GetComponent<Card>();
        _activeCards = new List<Card>();
        _activeCardObjects = new List<GameObject>();
        _soldOutMarkers = new List<GameObject>();
        _shopDeck = new Stack<String>();
        _discarded = new Stack<String>();
        GenerateCatalog();
        ShuffleShopDeck();
        _cg = GetComponent<CanvasGroup>();
        _leaveButton = bg.Find("LeaveShop").GetComponent<Button>();
        _aetherText = bg.Find("AetherCount").transform.Find("AetherText").GetComponent<TMP_Text>();
        _cardParent = bg.Find("CardPositions");

        _magicCircle[0] = transform.Find("MagicCircle_Back").gameObject;
        _magicCircle[1] = transform.Find("MagicCircle_Front").gameObject;
        _coreFilled = bg.Find("AetherCount").transform.Find("Filled").gameObject;
        
        var positions = _cardParent.GetComponentsInChildren<RectTransform>();
        _positions = new RectTransform[positions.Length];
        int j = 0;
        for (int i = _positions.Length - 1; i >= 0; i--)
        {
            _positions[i] = positions[j];
            j++;
        }
        BuysRemaining = -1;
        FreeBuysRemaining = 0;
        Previewing = false;
        _showShopButton = GameObject.Find("ShowShopButton").GetComponent<Button>();
        _showShopButton.onClick.AddListener(()=>Preview());
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
            //_catalog.Remove(c.CardName);
            RemoveCard(c);
            StartCoroutine(DealNewCard());
            DeckManager.Instance.Discard(c);
        }

        if (BuysRemaining != -1)
            BuysRemaining--;
        
        updateStatus();
    }

    public void LoadShop()
    {
        if (TutorialManager.Instance != null && TutorialManager.Instance.TutorialStep == 2)
            TutorialManager.Instance.Step();
        
        updateStatus();

        StartCoroutine(LoadBuyMenu());
    }

    public void Preview()
    {
        Previewing = true;
        LoadShop();
    }
    
    public IEnumerator LoadBuyMenu()
    {
        if (BattleManager.Instance.BattleState == BattleStates.GameOver)
            yield break;
        _lastState = BattleManager.Instance.BattleState;
        BattleManager.Instance.BattleState = BattleStates.BuyingCards;
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
        _activeCardObjects.Clear();
        
        for (int i = 0; i < DealAmt; i++)
        {
            StartCoroutine(DealNewCard());
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

        for (int i = _activeCards.Count - 1; i >= 0; i--)
        {
            _shopDeck.Push(_activeCards[i].CardName);
        }
        
        if (_lastState == BattleStates.Battle && !Previewing)
        {
            _lastState = BattleStates.ChoosingAction;
            Tween rotate = _activeCards[0].transform.DOMoveX(-200, 0.5f);
            yield return rotate.WaitForCompletion();
            BattleManager.Instance.SetConfirmOn();
            RotateRow();
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
        
        
        BattleManager.Instance.BattleState = _lastState;
       
        BuysRemaining = -1; //By default you have infinite buys
        FreeBuysRemaining = 0;
        Previewing = false;

        Utils.DestroyCardPreview();
    }

    private IEnumerator DealNewCard()
    {
        if(_shopDeck.Count == 0)
            ShuffleShopDeck();
        GameObject activeCardGO = Instantiate(Utils.LoadCard(_shopDeck.Pop()), DeckPos.position, Quaternion.identity, _cardParent);
        Card activeCard = activeCardGO.GetComponent<Card>();
        activeCard.Purchasable = true;
        activeCard.ShowFullSize = true;
        _activeCards.Add(activeCard);

        activeCardGO.GetComponent<CardEventManager>().enabled = false;
            
        _activeCardObjects.Add(activeCardGO);
        
        if(_activeCardObjects.Count > 1)
            activeCardGO.transform.SetSiblingIndex(_activeCardObjects[_activeCardObjects.Count-2].transform.GetSiblingIndex());

        Sequence dealTween = DOTween.Sequence();
        dealTween.Append(activeCardGO.transform.DOMove(CurrentCardPos(_activeCardObjects.Count-1).position, 0.5f, false));
        dealTween.Join(activeCardGO.transform.DORotate(CurrentCardPos(_activeCardObjects.Count-1).rotation.eulerAngles, 0.5f));
        dealTween.Join(activeCardGO.transform.DOScale(CurrentCardPos(_activeCardObjects.Count-1).localScale, 0.5f));

        yield return dealTween.WaitForCompletion();
        activeCardGO.GetComponent<CardEventManager>().enabled = true;
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

    public void RotateRow()
    {
        _discarded.Push(_shopDeck.Pop());
    }
    
    private RectTransform CurrentCardPos(int i)
    {
        return _positions[i];
    }
    private void Update()
    {
        _aetherText.text = BattleManager.Instance.CurrentAether.ToString();
        
        if (BuysRemaining != -1)
            _aetherText.text += "\nBuys Remaining: " + BuysRemaining;

        if (FreeBuysRemaining != 0)
            _aetherText.text += "\nFree buys Remaining: " + FreeBuysRemaining;
    }

    public void RemoveCard(Card c)
    {
        int cInd = _activeCards.IndexOf(c);
        Card nextC = null;

        if (_activeCards.Count > 1 && cInd != _activeCards.Count - 1)
            nextC = _activeCards[cInd + 1];

        if (cInd == _activeCards.Count)
            return;
        if (_activeCards.Count == 0)
            return;
        //nextC.transform.position = CurrentCardPos(cInd);
        Sequence dealTween = DOTween.Sequence();
        while (cInd < _activeCards.Count - 1)
        {
            cInd++;
            nextC = _activeCards[cInd];
            dealTween.Append(nextC.transform.DOMove(CurrentCardPos(cInd-1).position, 0.2f, false));
            dealTween.Join(nextC.transform.DORotate(CurrentCardPos(cInd-1).rotation.eulerAngles, 0.2f));
            dealTween.Join(nextC.transform.DOScale(CurrentCardPos(cInd-1).localScale, 0.2f));
        }
        
        _activeCards.Remove(c);
    }

    //TODO: Actually Implement this, card selection can be different based on level of enemy, area, etc
    public void GenerateCatalog()
    {
        if (OverrideStore)
            return;
        
        _catalog = new List<string>();
        
        for (int i = 0; i < 20; i++)
        {
            _catalog.Add(CardDirectory.GetRandomCard(Utils.GetRandomArchetype(), Utils.GetRandomRarity(), true));
        }
    }

    public void LoadNewCatalog(List<string> catalog)
    {
        _catalog = catalog;
        ShuffleShopDeck();
    }

    private void updateStatus()
    {
        if (BattleManager.Instance.CurrentAether > 0)
        {
            for (int i = 0; i < _magicCircle.Length; i++)
            {
                _magicCircle[i].SetActive(true);
            }
            _coreFilled.SetActive(true);
        }
        else
        {
            for (int i = 0; i < _magicCircle.Length; i++)
            {
                _magicCircle[i].SetActive(false);
            }
            _coreFilled.SetActive(false);
        }
    }
}
