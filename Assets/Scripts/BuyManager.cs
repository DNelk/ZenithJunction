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

    //Vars
    private CanvasGroup _cg;
    public Transform DeckPos;
    public float XInterval;
    private Button _leaveButton;
    private List<GameObject> _activeCardObjects;
    private List<Card> _activeCards;
    [SerializeField] private List<String> _catalog = new List<String>();
    private Stack<String> _shopDeck;
    private Transform _cardParent;
    private Text _steamText;
    
    //Freebies
    private Card _endlessAtk;
    private Card _endlessSteam;
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
        _endlessSteam = transform.Find("EndlessSteam").GetComponent<Card>();
        _activeCards = new List<Card>();
        _activeCardObjects = new List<GameObject>();
        _soldOutMarkers = new List<GameObject>();
        _shopDeck = new Stack<String>();
        ShuffleShopDeck();
        _cg = GetComponent<CanvasGroup>();
        _leaveButton = transform.Find("LeaveShop").GetComponent<Button>();
        _leaveButton.onClick.AddListener(LeaveShop);
        _cardParent = transform.Find("Cards").transform;
        _steamText = transform.Find("SteamText").GetComponent<Text>();
    }

    //Buy a card we click on
    public void BuyCard(Card c)
    {
        if (BattleManager.Instance.CurrentSteam < c.BuyCost)
            return;
        BattleManager.Instance.CurrentSteam -= c.BuyCost;
        if (c == _endlessAtk)
        {
            _soldOutMarkers.Add(Instantiate(Resources.Load<GameObject>("Prefabs/SoldOutCard"), c.transform.position, Quaternion.identity, transform));
            DeckManager.Instance.Discard(Instantiate(Resources.Load<GameObject>("Prefabs/Cards/Strike").GetComponent<Card>()));
        }
        else if (c == _endlessSteam)
        {
            _soldOutMarkers.Add(Instantiate(Resources.Load<GameObject>("Prefabs/SoldOutCard"), c.transform.position, Quaternion.identity, transform));
            DeckManager.Instance.Discard(Instantiate(Resources.Load<GameObject>("Prefabs/Cards/Boil").GetComponent<Card>()));
        }
        else
        {
            c.Purchasable = false;
            _activeCardObjects.Remove(c.gameObject);
            _catalog.Remove(c.CardName);
            StartCoroutine(DealNewCard(c.transform.position.x, c.transform.GetSiblingIndex()));
            DeckManager.Instance.Discard(c);
        }
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

            StartCoroutine(DealNewCard(x,i));
        }
    }

    public void LeaveShop()
    {
        StartCoroutine(LeaveBuyMenu());
    }

    private IEnumerator LeaveBuyMenu()
    {
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
    }

    private IEnumerator DealNewCard(float xPosition, int siblingIndex)
    {
        GameObject activeCardGO = Instantiate(Resources.Load<GameObject>("prefabs/cards/" + _shopDeck.Pop().Replace(" ", String.Empty)), DeckPos.position, Quaternion.identity, _cardParent);
        Card activeCard = activeCardGO.GetComponent<Card>();
        activeCardGO.transform.SetSiblingIndex(siblingIndex);
        activeCard.Purchasable = true;
        _activeCards.Add(activeCard);
            
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
        _steamText.text = "Steam Remaining: " + BattleManager.Instance.CurrentSteam;
    }
}
