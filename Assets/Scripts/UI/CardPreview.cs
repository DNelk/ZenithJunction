using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPreview : MonoBehaviour
{
    private GameObject _myCard;
    private CanvasGroup _cg;

    [SerializeField] protected Sprite _commonSprite; public Sprite CommonSprite => _commonSprite;
    [SerializeField] protected Sprite _uncommonSprite; public Sprite UncommonSprite => _uncommonSprite;
    [SerializeField] protected Sprite _rareSprite; public Sprite RareSprite => _rareSprite;
    [SerializeField] protected Sprite _ultraRareSprite; public Sprite UltraRareSprite => _ultraRareSprite;
    
    //Card UI
    protected TMP_Text[] u_cardName;
    protected Image u_cardName_color;
    protected Image u_rarity;
    protected TMP_Text[] u_type;
    protected Image u_type_color;
    protected TMP_Text u_buyCost;
    protected GameObject u_attackValue;
    protected GameObject u_aetherValue;
    protected GameObject u_moveValue;
    protected Image[] u_aetherCost;
    protected Text u_aetherCost_X;
    protected Image[] u_range;
    protected TMP_Text u_bodyText;
    protected Image u_image;
    protected Image u_glow;
    protected CanvasGroup u_cg;

    private void Awake()
    {
        _cg = GetComponent<CanvasGroup>();
        _cg.alpha = 0;

        init();
    }

    protected void init()
    {
        u_cardName = transform.Find("NameBanner").GetComponentsInChildren<TMP_Text>();
        u_cardName_color = transform.Find("NameBanner").GetComponent<Image>();
        u_rarity = transform.Find("Rarity").GetComponent<Image>();
        u_type = transform.Find("Type").GetComponentsInChildren<TMP_Text>();
        u_type_color = transform.Find("Type").GetComponent<Image>();
        u_buyCost = transform.Find("Cost").transform.Find("BuyCost").GetComponent<TMP_Text>();
        u_attackValue = transform.Find("Parameter").transform.Find("Parameter_Attack").gameObject;
        u_aetherValue = transform.Find("Parameter").transform.Find("Parameter_Aether").gameObject;
        u_moveValue = transform.Find("Parameter").transform.Find("Parameter_Move").gameObject;
        u_aetherCost = transform.Find("Aether_Cost").GetComponentsInChildren<Image>();
        u_aetherCost_X = u_aetherCost[0].transform.Find("AetherCost_Xnumber").GetComponent<Text>();
        u_range = transform.Find("Range").GetComponentsInChildren<Image>();
        u_bodyText = transform.Find("BodyText").GetComponent<TMP_Text>();
        u_image = transform.Find("CardImage").GetComponent<Image>();
    }
    
    public void SetCard(Card c)
    {
        /*_myCard = Instantiate(Resources.Load<GameObject>("Prefabs/Cards/" + c.CardName.Replace(" ", String.Empty)), transform.GetChild(0));
        if(BattleManager.Instance != null && BattleManager.Instance.BattleState == BattleStates.BuyingCards)
            _myCard.GetComponent<Card>().Purchasable = true;*/

        //set name
        for (int i = 0; i < u_cardName.Length; i++)
        {
            u_cardName[i].text = c.CardName;
        }

        //set rarity
        switch (c.CardRarity)
        {
            case CardRarities.Common:
                u_rarity.sprite = _commonSprite;
                break;
            case CardRarities.Uncommon:
                u_rarity.sprite = _uncommonSprite;
                break;
            case CardRarities.Rare:
                u_rarity.sprite = _rareSprite;
                break;
            case CardRarities.UltraRare:
                u_rarity.sprite = _ultraRareSprite;
                break;
        }
        
        //set type

        for (int i = 0; i < u_type.Length; i++)
        {
            switch (c.CardType)
            {
                case CardTypes.Attack:
                    u_type[i].text = "Attack";
                    u_type_color.color = new Color(0.752f, 0.098f, 0);
                    u_cardName_color.color = new Color(0.752f, 0.098f, 0);
                    break;
                case CardTypes.Aether:
                    u_type[i].text = "Aether";
                    u_type_color.color = new Color(0.2f, 0.18f, 0.58f); 
                    u_cardName_color.color = new Color(0.2f, 0.18f, 0.58f);
                    break;
                case CardTypes.Special:
                    u_type[i].text = "Special";
                    u_type_color.color = new Color(0.658f,0.282f,0.627f); 
                    u_cardName_color.color = new Color(0.658f,0.282f,0.627f);
                    break;
                case CardTypes.Movement:
                    u_type[i].text = "Movement";
                    u_type_color.color = new Color(0.956f,0.749f,0.031f); 
                    u_cardName_color.color = new Color(0.956f,0.749f,0.031f);
                    break;
            }
        }

        //set buyCost
        
        //set parameter
        //assign Parameter
        if (c.PowerValue > 0)
        {
            u_attackValue.SetActive(true);
            u_attackValue.GetComponentInChildren<TMP_Text>().text = c.PowerValue.ToString();
            
            if (c.PowerValue == 1)
                u_attackValue.GetComponentInChildren<TMP_Text>().gameObject.transform.position -= new Vector3(2.5f,0,0);
        }
        if (c.AetherValue > 0)
        {
            u_aetherValue.SetActive(true);
            u_aetherValue.GetComponentInChildren<TMP_Text>().text = c.AetherValue.ToString();
            
            if (c.AetherValue == 1)
                u_aetherValue.GetComponentInChildren<TMP_Text>().gameObject.transform.position -= new Vector3(2.5f,0,0);
        }
        if (c.MoveValue > 0)
        {
            u_moveValue.SetActive(true);
            u_moveValue.GetComponentInChildren<TMP_Text>().text = c.MoveValue.ToString();
                        
            if (c.MoveValue == 1)
                u_moveValue.GetComponentInChildren<TMP_Text>().gameObject.transform.position -= new Vector3(2.5f,0,0);
        }
        
        //if it's an X card
        if (c.AetherCost == -1) //-1 is X 
        {
            u_aetherCost[0].color = Color.white; //make the first aether cost symbol active
            u_aetherCost_X.color = Color.white; //set the X text to be active
            u_aetherCost[0].transform.localScale *= 1.3f; //scaling them to be bigger
        }
        else if (c.AetherCost > 0)
        {
            for (int i = 0; i < c.AetherCost; i++)
            {
                u_aetherCost[i].color = Color.white; //set the symbol active depend on aether cost
            }
        }
        else if (c.AetherCost == 0) {}
            //nothing happen
        
        //set range
        //Debug.Log(c.Range);
        switch (c.Range)
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
        
        //set text
        //u_bodyText.text = Utils.ReplaceWithSymbols(c.CardText);
        u_bodyText.text = c.CardText;
        
        //set image
        if (c.CardImage != null)
        {
            u_image.sprite = c.CardImage;
            u_image.color = Color.white;
        }


        FadeIn();
    }

    private void FadeIn()
    {
        _cg.DOFade(1, 0.2f);
    }

    public void Destroy()
    {
        _cg.DOFade(0, 0.1f).OnComplete(() =>
        {
            Destroy(gameObject);
            Utils.CurrentPreview = null;
        });
    }
}
