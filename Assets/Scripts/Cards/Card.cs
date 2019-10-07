using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class Card : MonoBehaviour
{
    //Card Variables
    [SerializeField] protected string _cardName; public String CardName => _cardName;
    [SerializeField] protected CardTypes _cardType; public CardTypes CardType => _cardType;
    [SerializeField] protected int _buyCost; public int BuyCost => _buyCost;
    [SerializeField] protected int _aetherCost;
    [SerializeField] protected string _cardText;
    [SerializeField] protected int _aetherValue;
    [SerializeField] protected int _atkValue;
     public int AtkMod; //Modified attack value
    [HideInInspector] public int AetherMod; //Modified attack value
    [SerializeField] protected int _priority; public int Priority => _priority;
    public bool Purchasable = false;
    [HideInInspector]public int XValue = 0;
    [HideInInspector] public bool Tweening = false;
    [HideInInspector] public bool IsPreview = false;
    public bool IsXCost => _aetherCost == -1;
    
    
    //UI
    [HideInInspector] public bool Dragging = false;
    protected Engine _myEngine; 
    public Engine Engine
    {
        get => _myEngine;
        set => _myEngine = value;
    }


    //Card UI
    protected Image u_cardBackground;
    protected Text u_cardName;
    protected Text u_type;
    protected Text u_buyCost;
    protected Text u_aetherCost;
    protected Text u_bodyText;
    protected Image u_image;
    protected Image u_glow;

    // Called when object is created
    protected void Awake()
    {
        Init();
        AssignUI();
    }

    protected void Init()
    {
        u_cardBackground = transform.Find("Back").GetComponent<Image>();
        u_cardName = transform.Find("CardName").GetComponent<Text>();
        u_type = transform.Find("TypeLine").GetComponent<Text>();
        u_buyCost = transform.Find("BuyCost").GetComponent<Text>();
        u_aetherCost = transform.Find("AetherCost").GetComponent<Text>();
        u_bodyText = transform.Find("BodyText").GetComponent<Text>();
        u_image = transform.Find("Image").GetComponent<Image>();
        u_glow = transform.Find("Glow").GetComponent<Image>();
        u_glow.color = Color.clear;
    }
    
    //Execute a card's unique text
    public virtual void Execute()
    {

    }

    public virtual void ExecuteFailed()
    {
        
    }

    protected void AssignUI()
    {
        u_cardName.text = _cardName;
        u_type.text = Enum.GetName(typeof(CardTypes), _cardType);
        u_buyCost.text = _buyCost.ToString();
        u_aetherCost.text = _aetherCost.ToString();
        if (_aetherCost == -1) //-1 is X
            u_aetherCost.text = "X";
        u_bodyText.text = _cardText;
    }

    #region Combat and Effects

    public void ReadyCard()
    {
        AtkMod = _atkValue;
        AetherMod = _aetherValue;
    }

    public void SetEngine(Color glowColor, Transform parent)
    {
        u_glow.color = glowColor;
        transform.SetParent(parent);
    }
    
    public void SetEngine(Color glowColor, Transform parent, Vector3 position)
    {
        u_glow.color = glowColor;
        transform.SetParent(parent);
        Tweening = true;
        transform.DOMove(position, 0.5f).OnComplete(() => Tweening = false);
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
    #endregion
    
}

public enum CardTypes
{
    Attack,
    Aether,
    Special
}
