using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    //Card Variables
    [SerializeField] protected string _cardName; public String CardName => _cardName;
    [SerializeField] protected CardTypes _cardType; public CardTypes CardType => _cardType;
    [SerializeField] protected int _buyCost; public int BuyCost => _buyCost;
    [SerializeField] protected int _castCost;
    [SerializeField] protected string _cardText;
    [SerializeField] protected int _focusValue;
    [SerializeField] protected int _atkValue;
    [HideInInspector] public int AtkMod; //Modified attack value
    [HideInInspector] public int FocusMod; //Modified attack value
    [SerializeField] protected int _priority; public int Priority => _priority;
    public bool Purchasable = false;
    
    protected StackManager _manager; 
    public StackManager Manager
    {
        get => _manager;
        set => _manager = value;
    }


    //Card UI
    protected Image u_cardBackground;
    protected Text u_cardName;
    protected Text u_type;
    protected Text u_buyCost;
    protected Text u_castCost;
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
        u_castCost = transform.Find("CastCost").GetComponent<Text>();
        u_bodyText = transform.Find("BodyText").GetComponent<Text>();
        u_image = transform.Find("Image").GetComponent<Image>();
        u_glow = transform.Find("Glow").GetComponent<Image>();
        u_glow.color = Color.clear;
    }
    
    //Execute a card's unique text
    public virtual void Execute()
    {

    }

    protected void AssignUI()
    {
        u_cardName.text = _cardName;
        u_type.text = Enum.GetName(typeof(CardTypes), _cardType);
        u_buyCost.text = _buyCost.ToString();
        u_castCost.text = _castCost.ToString();
        u_bodyText.text = _cardText;
    }

    #region Combat and Effects

    public void ReadyCard()
    {
        AtkMod = _atkValue;
        FocusMod = _focusValue;
    }

    public void SetStack(Color glowColor, Transform parent)
    {
        u_glow.color = glowColor;
        transform.SetParent(parent);
    }

    //Pay focus cost for spells
    public bool PayForCard()
    {
        int remainingCost = _castCost;
        
        if (_castCost == 0)
            return true;
        
        foreach (Card c in _manager.Stack)
        {
            if (c.FocusMod > 0)
            {
                //If we need to drain it completely
                if (remainingCost >= c.FocusMod)
                {
                    remainingCost -= c.FocusMod;
                    c.FocusMod = 0;
                }
                //It will have leftover focus after paying
                else if (remainingCost < c.FocusMod)
                {
                    remainingCost -= c.FocusMod;
                    c.FocusMod -= remainingCost;
                }
                if (remainingCost <= 0)
                    return true;
            }
        }

        return false;
    }
    #endregion
    
}

public enum CardTypes
{
    Attack,
    Focus,
    Spell
}
