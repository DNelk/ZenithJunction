using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Card : MonoBehaviour
{
    //Card Variables
    [SerializeField] protected string _cardName; public String CardName => _cardName;
    [SerializeField] protected CardTypes _cardType; public CardTypes CardType => _cardType;
    [SerializeField] protected int _buyCost; public int BuyCost => _buyCost;
    [SerializeField] protected int _aetherCost; public int AetherCost => _aetherCost;
    [SerializeField] protected string _cardText;
    [SerializeField] protected int _aetherValue;
    [SerializeField] protected int _powerValue;
    [SerializeField] protected AttackRange _range = AttackRange.Null;
    [SerializeField] protected int _moveValue;
    [HideInInspector] public int PowerTotal; //Modified attack value
    [HideInInspector] public int AetherTotal; //Modified aether value
    [HideInInspector] public int MoveTotal; //Totalified aether value
    
    //Gameplay
    [HideInInspector] public int XValue = 0;
    public bool IsXCost => _aetherCost == -1;
    [HideInInspector] public bool TrashThis = false; //Cards that are to be trashed are instead removed from the deck until end of the battle

    //Engines
    protected Engine _myEngine; 
    public Engine Engine
    {
        get => _myEngine;
        set => _myEngine = value;
    }
    
    [SerializeField] protected int _priority; public int Priority => _priority;
    
    //Store
    [SerializeField] private bool _purchasable = false;
    public bool Purchasable
    {
        get => _purchasable;
        set
        { 
            _purchasable = value;
            AssignUI();
        }
    }
    
    //Events & UX
    private Vector3 _initialScale;
    private CardEventManager _eventManager;
    [HideInInspector] public bool Tweening = false;
    [HideInInspector] public bool IsPreview = false;
    [HideInInspector] public bool Dragging = false;
    private bool _equipped = false;
    public bool Equipped
    {
        get => _equipped;
        set
        {
            _equipped = value;
            AssignUI();
        }
    }

    //Card UI
    protected Image u_cardBackground;
    protected TMP_Text u_cardName;
    protected TMP_Text u_type;
    protected TMP_Text u_buyCost;
    protected TMP_Text u_aetherCost;
    protected TMP_Text u_bodyText;
    protected Image u_image;
    protected Image u_glow;
    protected CanvasGroup u_cg;

    // Called when object is created
    protected void Awake()
    {
        Init();
        AssignUI();
    }

    private void Start()
    {
        _initialScale = transform.localScale;
    }

    protected void Init()
    {
        u_cardBackground = transform.Find("Back").GetComponent<Image>();
        u_cardName = transform.Find("NameBanner").transform.Find("CardName").GetComponent<TMP_Text>();
        u_type = transform.Find("Type").transform.Find("TypeLine").GetComponent<TMP_Text>();
        u_buyCost = transform.Find("Cost").transform.Find("BuyCost").GetComponent<TMP_Text>();
        u_aetherCost = transform.Find("Aether").transform.Find("AetherCost").GetComponent<TMP_Text>();
        u_bodyText = transform.Find("BodyText").GetComponent<TMP_Text>();
        u_image = transform.Find("Image").GetComponent<Image>();
        //u_glow = transform.Find("Glow").GetComponent<Image>();
        //u_glow.color = Color.clear;
        _initialScale = transform.localScale;
        _eventManager = GetComponent<CardEventManager>();
        u_cg = GetComponent<CanvasGroup>();
    }
    
    //Execute a card's unique text
    public virtual void Execute()
    {

    }

    public virtual void ExecuteFailed()
    {
        PowerTotal = 0;
        AetherTotal = 0;
        MoveTotal = 0;
    }

    protected void AssignUI()
    {
        u_cardName.text = _cardName;
        
        switch (CardType)
        {
            case CardTypes.Attack:
                u_cardBackground.color = new Color(0.9f, 0.6f, 0.6f);
                break;
            case CardTypes.Aether:
                u_cardBackground.color = new Color(0.7f, 1f, 1f);
                break;
            case CardTypes.Special:
                u_cardBackground.color = new Color(0.8f,0.6f,1f);
                break;
            case CardTypes.Movement:
                u_cardBackground.color = new Color(0.5f, 0.5f, 0f);
                break;
                
        }
        
        u_type.text = Enum.GetName(typeof(CardTypes), _cardType);

        switch (_range)
        {
            case AttackRange.Melee:
                u_type.text = "Melee " + u_type.text;
                break;
            case AttackRange.Short:
                u_type.text = "Ranged " + u_type.text + " - Short";
                u_cardBackground.color = new Color(0.8f, 0.3f, 0.1f);
                break;
            case AttackRange.Long:
                u_type.text = "Ranged " + u_type.text + " - Long";
                u_cardBackground.color = new Color(0.6f, 0.3f, 0.3f);
                break;
            default:
                break;
        }
        
        u_buyCost.text = _buyCost.ToString();
        if (_purchasable)
            u_buyCost.transform.parent.gameObject.SetActive(true);
        else
            u_buyCost.transform.parent.gameObject.SetActive(false);
        
        u_aetherCost.text = _aetherCost.ToString();
        if (_aetherCost == -1) //-1 is X
            u_aetherCost.text = "X";
        else if (_aetherCost == 0)
            u_aetherCost.transform.parent.gameObject.SetActive(false);
        
        u_bodyText.text = Utils.ReplaceWithSymbols(_cardText);

        if (_equipped)
            u_cg.alpha = 0.5f;
        else
            u_cg.alpha = 1f;
    }
    

    #region Combat and Effects

    public void ReadyCard()
    {
        PowerTotal = _powerValue;
        AetherTotal = _aetherValue;
        MoveTotal = _moveValue;
    }

    public void SetEngine(Transform parent)
    {
        transform.SetParent(parent);
        if (_eventManager.BaseScale != Vector3.zero)
            _eventManager.BaseScale = _initialScale;
        else
            transform.localScale = _initialScale;
    }
    
    public void SetEngine(Transform parent, Vector3 position, Vector3 scale)
    {
        // u_glow.color = glowColor;
        transform.SetParent(parent);
        Tweening = true;
        transform.DOMove(position, 0.5f).OnComplete(() => Tweening = false);
        transform.DOScale( scale.x * 9, 0.5f);
    }

    //Pay aether cost for spells
    public bool PayForCard()
    {
        int remainingCost = _aetherCost;
        
        if (_aetherCost == 0)
            return true;
        if (_myEngine.AetherTotal >= remainingCost)
        {
            _myEngine.AetherTotal -= remainingCost;
            return true;
        }

        return false;
    }

    public int CalculateAttackTotalWithPosition()
    {
        int distance = Mathf.Abs(BattleManager.Instance.Player.Position - BattleManager.Instance.CurrentEnemy.Position);
        float damageMod = 1;
        switch (_range)
        {
            //Melee Range Attacks only work right next to target
            case AttackRange.Melee:
                if (distance > 0)
                    damageMod = 0;
                break;
            //Short ranged attacks are weaker further away
            case AttackRange.Short:
                if (distance >= 2) 
                    damageMod = 0.5f;
                break;
            //Long range attacks are only good far away
            case AttackRange.Long:
                if (distance == 0)
                    damageMod = 0;
                break;
            default:
                damageMod = 1;
                break;
        }

        return (int)(PowerTotal * damageMod);
    }
    #endregion
    
    public virtual bool IsAttackInRange()
    {
        int distance = Mathf.Abs(BattleManager.Instance.Player.Position - BattleManager.Instance.CurrentEnemy.Position);
        switch (_range)
        {
            case AttackRange.Melee:
                if (distance != 0)
                    return false;
                break;
            case AttackRange.Short:
                if (distance == 2)
                    return false;
                break;
            case AttackRange.Long:
                if (distance == 0)
                    return false;
                break;
            default:
                break;
        }
        return true;
    }
}

public enum CardTypes
{
    Attack,
    Aether,
    Special,
    Movement
}


