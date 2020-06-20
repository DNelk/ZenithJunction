using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Card : MonoBehaviour
{
    //Card Variables
    [SerializeField] protected string _cardName; public String CardName => _cardName;
    [SerializeField] protected CardTypes _cardType;

    public CardTypes CardType
    {
        get => _cardType;
        set => _cardType = value;
    }
    
    [SerializeField] protected CardRarities _cardRarities; public CardRarities CardRarity => _cardRarities;
    [SerializeField] protected Sprite _cardImage; public Sprite CardImage => _cardImage;
    [SerializeField] protected int _buyCost; public int BuyCost => _buyCost;
    [SerializeField] protected int _aetherCost; public int AetherCost => _aetherCost;
    [SerializeField] protected string _cardText; public string CardText => _cardText;
    [SerializeField] protected int _aetherValue; public int AetherValue => _aetherValue;
    [SerializeField] protected int _powerValue;
    public int PowerValue
    {
        get => _powerValue;
        set => _powerValue = value;
    }

    
    [SerializeField] protected AttackRange _range = AttackRange.Null;
    public AttackRange Range
    {
        get => _range;
        set => _range = value;
    }

    [SerializeField] protected int _moveValue; public int MoveValue => _moveValue;
    [HideInInspector] public int PowerTotal; //Modified attack value
    [HideInInspector] public int AetherTotal; //Modified aether value
    [HideInInspector] public int MoveTotal; //Totalified aether value
    
    public List<Stat> StatMods = new List<Stat>();
    public List<Stat> EnemyStatMods = new List<Stat>();



    //Gameplay
    [HideInInspector] public int XValue = 0;
    public bool IsXCost => _aetherCost == -1;
    [HideInInspector] public bool TrashThis = false; //Cards that are to be trashed are instead removed from the deck until end of the battle

    //Engines
    protected Engine _myEngine;
    public Engine pendingEngine;
    
    public Engine Engine
    {
        get => _myEngine;
        set => _myEngine = value;
    }
    [HideInInspector] public bool _inSlot;
    
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
    //Events & UX
    private Vector3 _initialScale;
    [HideInInspector] public CardEventManager _eventManager;
    [HideInInspector] public bool Tweening = false;
    [HideInInspector] public bool IsPreview = false;
    [HideInInspector] public bool Dragging = false;
    [HideInInspector] public int MyIndex = 0;
    [HideInInspector] public bool InActive = false;
    [HideInInspector] public Collider2D MyCol;
    [HideInInspector] public GameObject MyCheatImg;
    [HideInInspector] public int MyEngineIndex = 0;
    
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
    protected Image u_rarity;
    protected TMP_Text u_type;
    protected Image u_TypeAura;
    protected Image u_type_color;
    protected TMP_Text u_buyCost;
    protected GameObject u_attackValue;
    protected GameObject u_aetherValue;
    protected GameObject u_moveValue;
    protected Image[] u_aetherCost;
    protected Text u_aetherCost_X;
    protected Text u_aetherCost_Overflow;
    protected Image[] u_range;
    protected TMP_Text u_bodyText;
    protected Image u_image;
    protected Image u_glow;
    protected CanvasGroup u_cg;
    private ParticleSystem u_particle;
    //private Color u_particleColor;

    //Fullsize
    [SerializeField] protected bool _fullSize = false;
    public bool ShowFullSize
    {
        get => _fullSize;
        set
        { 
            _fullSize = value;
            Init();
            AssignUI();
        }
    }

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
        Transform parent;
        
        //determine if small card or fullsize card
        if (_fullSize)
        {
            parent = transform.Find("FullSize");
            transform.Find("Minimized").gameObject.SetActive(false);
            u_TypeAura = null;
            
            //get cheat IMG
            MyCheatImg = parent.transform.Find("CheatImg").gameObject;
        }
        else
        {
            parent = transform.Find("Minimized");
            transform.Find("FullSize").gameObject.SetActive(false);
            u_TypeAura = parent.Find("Aura").GetComponent<Image>();
            
            //get cheat IMG
            MyCheatImg = parent.transform.Find("CheatImg").gameObject;
            MyCheatImg.SetActive(false);
        }
        
        //assign other UI
        parent.gameObject.SetActive(true);
        u_cardBackground = parent.Find("Back").GetComponent<Image>();
        u_cardName = parent.Find("NameBanner").transform.Find("CardName").GetComponent<TMP_Text>();
        u_rarity = parent.Find("Rarity").GetComponent<Image>();
        u_type = parent.Find("Type").transform.Find("TypeLine").GetComponent<TMP_Text>();
        u_type_color = parent.Find("Type").GetComponent<Image>();
        if (_fullSize) u_buyCost = parent.Find("Cost").transform.Find("BuyCost").GetComponent<TMP_Text>();
        u_attackValue = parent.Find("Parameter").transform.Find("Parameter_Attack").gameObject;
        u_aetherValue = parent.Find("Parameter").transform.Find("Parameter_Aether").gameObject;
        u_moveValue = parent.Find("Parameter").transform.Find("Parameter_Move").gameObject;
        u_aetherCost = parent.Find("Aether_Cost").GetComponentsInChildren<Image>();
        u_aetherCost_X = u_aetherCost[0].transform.Find("AetherCost_Xnumber").GetComponent<Text>();
        u_aetherCost_Overflow = u_aetherCost[0].transform.Find("AetherCost_OverflowNumber").GetComponent<Text>();
        u_range = parent.Find("Range").GetComponentsInChildren<Image>();
        if (_fullSize) u_bodyText = parent.Find("BodyText").GetComponent<TMP_Text>();
        if (!_fullSize) u_particle = transform.Find("PuffyGlow").GetComponent<ParticleSystem>();
        u_image = parent.Find("Back").transform.Find("CardImage").GetComponent<Image>();
        //u_glow = transform.Find("Glow").GetComponent<Image>();
        //u_glow.color = Color.clear;
        _initialScale = transform.localScale;
        _eventManager = GetComponent<CardEventManager>();
        u_cg = GetComponent<CanvasGroup>();
        MyCol = GetComponent<Collider2D>();
    }
    
    //Execute a card's unique text
    public virtual void Execute()
    {
        //Apply Some Stats
        if (BattleManager.Instance != null && BattleManager.Instance.BattleState == BattleStates.Battle)
        {
            foreach (var stat in StatMods)
            {
                BattleManager.Instance.Player.ModifyStat(stat.StatType, stat.TurnsLeft, stat.Value, !stat.IsNew);
            }
            
            foreach (var stat in EnemyStatMods)
            {
                BattleManager.Instance.CurrentEnemy.ModifyStat(stat.StatType, stat.TurnsLeft, stat.Value, !stat.IsNew);
            }
        }
    }

    public virtual void ExecuteFailed()
    {
        PowerTotal = 0;
        AetherTotal = 0;
        MoveTotal = 0;
    }

    public void AssignUI()
    {
        u_cardName.text = _cardName;
        if (_fullSize)
            u_cardName.transform.parent.Find("CardName_Shadow").GetComponent<TMP_Text>().text = _cardName;
        
        //check type
        if (_fullSize)
        {
            Image nameBanner = transform.Find("FullSize").Find("NameBanner").GetComponent<Image>();
            switch (CardType)
            {
                case CardTypes.Attack:
                    nameBanner.color = new Color(0.752f, 0.098f, 0);
                    u_type_color.color = new Color(0.752f, 0.098f, 0);
                    break;
                case CardTypes.Aether:
                    nameBanner.color = new Color(0.2f, 0.18f, 0.58f);
                    u_type_color.color = new Color(0.2f, 0.18f, 0.58f);
                    break;
                case CardTypes.Special:
                    nameBanner.color = new Color(0.658f,0.282f,0.627f);
                    u_type_color.color = new Color(0.658f,0.282f,0.627f);
                    break;
                case CardTypes.Movement:
                    nameBanner.color = new Color(0.956f,0.749f,0.031f);
                    u_type_color.color = new Color(0.956f,0.749f,0.031f);
                    break;
                default:
                    break;
            }
        }
        else
        {
            Color typeColor = new Color(0, 0, 0);
            
            //typeAura mat
            u_TypeAura.material = new Material(u_TypeAura.material);
            int auraID = Shader.PropertyToID("Aura_Color");
            Color auraColor = new Color(0,0,0);
            
            //typeParticle Mat
            ParticleSystemRenderer particleRend = u_particle.GetComponent<ParticleSystemRenderer>();
            particleRend.material = new Material(particleRend.material);

            switch (CardType)
            {
                case CardTypes.Attack:
                    typeColor = new Color(0.752f, 0.098f, 0);
                    auraColor = new Color(0.9f,0,0);
                    particleRend.material.mainTexture = Resources.Load<Texture>("Sprites/CardParticle/Particle_Atk");
                    break;
                case CardTypes.Aether:
                    typeColor = new Color(0.2f, 0.18f, 0.58f);
                    auraColor = new Color(0,0.35f,1f);
                    particleRend.material.mainTexture = Resources.Load<Texture>("Sprites/CardParticle/Particle_Mana");
                    break;
                case CardTypes.Special:
                    typeColor = new Color(0.658f,0.282f,0.627f);
                    auraColor = new Color(0.8f,0,0.8f);
                    particleRend.material.mainTexture = Resources.Load<Texture>("Sprites/CardParticle/Particle_Special");
                    break;
                case CardTypes.Movement:
                    typeColor = new Color(0.956f,0.749f,0.031f);
                    auraColor = new Color(0.8f,0.7f,0);
                    particleRend.material.mainTexture = Resources.Load<Texture>("Sprites/CardParticle/Particle_Move");
                    break;
                default:
                    break;
            }

            //u_particleColor = typeColor;
            u_type_color.color = typeColor;
            u_TypeAura.material.SetColor(auraID, auraColor);
        }
        
        /*Gradient _inEngineColor = new Gradient();
        _inEngineColor.SetKeys(new GradientColorKey[]{new GradientColorKey(u_particleColor, 0.0f)}, 
            new GradientAlphaKey[]{new GradientAlphaKey(0.0f, 0.0f), new GradientAlphaKey(1.0f, 0.524f),new GradientAlphaKey(1.0f, 0.75f), new GradientAlphaKey(0.0f, 1.0f)});

        var smoke = u_particle.colorOverLifetime;
        smoke.color = _inEngineColor;*/

        //check rarity
        if(_fullSize)
        {
            switch (CardRarity)
            {
                case CardRarities.Common:
                    u_rarity.sprite = Resources.Load<Sprite>("Sprites/Rarity_Common");
                    break;
                case CardRarities.Uncommon:
                    u_rarity.sprite = Resources.Load<Sprite>("Sprites/Rarity_Uncommon");
                    break;
                case CardRarities.Rare:
                    u_rarity.sprite = Resources.Load<Sprite>("Sprites/Rarity_Rare");
                    break;
                case CardRarities.UltraRare:
                    u_rarity.sprite = Resources.Load<Sprite>("Sprites/Rarity_UltraRare");
                    break;
                default:
                    break;
            }
        }
        else
        {
            /*switch (CardRarity)
            {
                case CardRarities.Common:
                    u_rarity.color = new Color(0, 0, 0);
                    break;
                case CardRarities.Uncommon:
                    u_rarity.color = new Color(0.7f, 0.7f, 0.7f);
                    u_rarity.color = new Color(0, 0, 0);
                    break;
                case CardRarities.Rare:
                    u_rarity.color = new Color(1, 0.8f, 0);
                    u_rarity.color = new Color(0, 0, 0);
                    break;
                case CardRarities.UltraRare:
                    u_rarity.color = new Color(0.8f, 0.5f, 1);
                    u_rarity.color = new Color(0, 0, 0);
                    break;
                default:
                    break;
            }*/
        }

        //check Image
        if (_cardImage != null) // check first there is the CardArt Ornot
            u_image.sprite = _cardImage;
        else
        {
            u_image.color = new Color(0.2f, 0.2f, 0.2f);
        }
        
        //check type_Text
        u_type.text = Enum.GetName(typeof(CardTypes), _cardType);
        if (_fullSize)
            u_type.transform.parent.Find("TypeLine_Shadow").GetComponent<TMP_Text>().text = u_type.text;


        //check Range
        switch (_range)
        {
            case AttackRange.Melee:
                break;
            case AttackRange.Short:
                u_range[0].color = Color.white;
                break;
            case AttackRange.Long:
                u_range[0].color = Color.white;
                u_range[1].color = Color.white;
                break;
            default:
                break;
        }

        if (_fullSize)
        {
            u_buyCost.text = _buyCost.ToString();
            if (_purchasable)
                u_buyCost.transform.parent.gameObject.SetActive(true);
            else
                u_buyCost.transform.parent.gameObject.SetActive(false);
        }

        //Aether Cost change

        //if it's an X card
        if (_aetherCost == -1) //-1 is X 
        {
            u_aetherCost[0].color = Color.white; //make the first aether cost symbol active
            u_aetherCost_X.color = Color.white; //set the X text to be active
            u_aetherCost[0].transform.localScale *= 1.3f; //scaling them to be bigger
        }
        else if (_aetherCost > 6)
        {
            u_aetherCost[0].color = Color.white; //make the first aether cost symbol active
            u_aetherCost_Overflow.color = Color.white; //set the X text to be active
            u_aetherCost_Overflow.text = _aetherCost.ToString();
            u_aetherCost[0].transform.localScale *= 1.3f; //scaling them to be bigger
        }
        else if (_aetherCost > 0)
        {
            for (int i = 0; i < _aetherCost; i++)
            {
                u_aetherCost[i].color = Color.white; //set the symbol active depend on aether cost
            }
        }
        else if (_aetherCost == 0)
        {
            //nothing happen
        }

        //set Text
        if (_fullSize)
            u_bodyText.text = Utils.ReplaceWithSymbols(_cardText);

        if (_equipped)
            u_cg.alpha = 0.5f;
        else
            u_cg.alpha = 1f;
        
        //assign Parameter
        if (_powerValue > 0)
        {
            u_attackValue.SetActive(true);
            u_attackValue.GetComponentInChildren<TMP_Text>().text = _powerValue.ToString();
        }
        if (_aetherValue > 0)
        {
            u_aetherValue.SetActive(true);
            u_aetherValue.GetComponentInChildren<TMP_Text>().text = _aetherValue.ToString();
        }
        if (_moveValue > 0)
        {
            u_moveValue.SetActive(true);
            u_moveValue.GetComponentInChildren<TMP_Text>().text = _moveValue.ToString();
        }
    }
    

    #region Combat and Effects

    public void ReadyCard()
    {
        PowerTotal = _powerValue;
        AetherTotal = _aetherValue;
        MoveTotal = _moveValue;
    }

    public void SetEngine(Transform parent, Vector3 position)
    {
        ParticleSystem cardGlow = _eventManager.Glow;
        turnCheatImageRaycast(false); //make it unable to perform any mouse event until the process is done
        
        //state of card
        transform.SetParent(parent); //change parent
        _inSlot = false; //not in deck slot anymore
        
        //turn on the aura
        if (!u_TypeAura.gameObject.activeSelf) SwitchTypeAura(true); 
        if (!cardGlow.gameObject.activeSelf) cardGlow.gameObject.SetActive(true);
        if (!cardGlow.isPlaying) cardGlow.Play();

        //change scale to small in case of being drag
        if (_eventManager.hovering)
        {
            transform.DOScale( _eventManager.InEngineScale * 1.5f, 0.2f); //change card size
            _eventManager.setParticleGlowSize(0.72f); //set particle size
        }
        else
        {
            transform.DOScale( _eventManager.InEngineScale, 0.2f); //change card size
            _eventManager.setParticleGlowSize(0.41f); //set particle size
        }
        
        transform.DOLocalMove(position, 0.5f).OnComplete(() => turnCheatImageRaycast(true));
        _eventManager.BaseScale = _eventManager.InEngineScale;

        //set other available engine off
        for (int i = 0; i < BattleManager.Instance.Engines.Length; i++)
        {
            if (BattleManager.Instance.Engines[i] != Engine) BattleManager.Instance.Engines[i].disselectGear();
        }
        
        //turn on myCheatImageForRaycastCheck
        Engine.turnMyCheatImageForRayCastCheck(true);
        
    }
    
    //use this function to put the card off engine back to its original hierachy
    public void OffEngine(Transform parent)
    {
        //change of card
        if (Dragging) 
        {
            transform.DOScale( _eventManager.OutEngineScale * 1.5f, 0.3f); //change while still dragging
        }
        else
        {
            transform.DOScale( _eventManager.OutEngineScale, 0.1f); //change back to original
            _eventManager.setParticleGlowSize(1f);
        }
        transform.SetParent(parent);
        _eventManager.BaseScale = _eventManager.OutEngineScale;
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

    public virtual int CalculateAttackTotalWithPosition(int overridePowerTotal = 0)
    {
        if (overridePowerTotal != 0)
            PowerTotal = overridePowerTotal;
        
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

    public void SwitchTypeAura(bool turn)
    {
        if (!_fullSize && u_TypeAura.gameObject.activeSelf != turn) u_TypeAura.gameObject.SetActive(turn);
    }

    public void turnCheatImageRaycast(bool turn)
    {
        MyCheatImg.SetActive(turn);
    }
}

public enum CardTypes
{
    Attack,
    Aether,
    Special,
    Movement
}

public enum CardRarities
{
    Common,
    Uncommon,
    Rare,
    UltraRare
}

public enum CardArchetype
{
    BigEcon,
    Melee,
    Ranged
}