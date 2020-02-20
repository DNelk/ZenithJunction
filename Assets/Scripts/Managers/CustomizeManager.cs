using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages loading player's card collection and customizing 
/// </summary>
public class CustomizeManager : MonoBehaviour
{
    public static CustomizeManager Instance;

    private Transform _content;
    private Transform _equipped;
    
    private List<Card> _cardsOnScreen;
    private List<EquippedCardIcon> _equippedCardIcons;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if(Instance != this)
            Destroy(gameObject);

    }

    private void Start()
    {
        GameManager.Instance.State = GameState.Customizing;
        _content = GameObject.FindWithTag("CustomizeScreenContent").transform;
        _equipped = GameObject.FindWithTag("CustomizeScreenSelected").transform;
        _cardsOnScreen = new List<Card>();
        _equippedCardIcons = new List<EquippedCardIcon>();
        
        if (!File.Exists(Application.persistentDataPath + "playercollection.save"))
        {
            PlayerCollection pc = new PlayerCollection();
            pc.Cards.Add("strike");
            pc.Cards.Add("strike");
            pc.Cards.Add("strike");
            pc.Cards.Add("strike");
            pc.Cards.Add("railcharge");
            pc.Cards.Add("allaboard");
            pc.Cards.Add("manaboil");
            pc.Cards.Add("manaboil");
            pc.Cards.Add("manaboil");
            pc.Cards.Add("DevRage");
        
            Utils.Save(pc, "playercollection");
        }

        PlayerCollection collection = Utils.Load<PlayerCollection>("playercollection");
        

        foreach (string c in collection.Cards)
        {
            GameObject go = Instantiate(Utils.LoadCard(c), _content);
            go.transform.localScale = Vector3.one * 70f;
            var go_c = go.GetComponent<Card>();
            go_c.ShowFullSize = true;
            _cardsOnScreen.Add(go_c);
        }
    
        //Create equipped list
        foreach (string e in collection.Equipped)
        {
            bool found = false;
            foreach (Card c in _cardsOnScreen)
            {
                if (c.CardName == e && !c.Equipped && !found)
                {
                    c.Equipped = true;
                    GameObject equipGO = Instantiate(Resources.Load<GameObject>("prefabs/UI/EquippedCardIcon"), _equipped);
                    EquippedCardIcon icon = equipGO.GetComponent<EquippedCardIcon>();
                    icon.CardName = e;
                    icon.CardObj = c.gameObject;
                    _equippedCardIcons.Add(icon);
                    found = true;
                }
            }
        }
    }
    
    public void SelectEquippedCard(Card c)
    {
        //Handle equipping cards
        //if list has space, equip, else dont

        if (_equippedCardIcons.Count >= 9 || c.Equipped)
            return;

        c.Equipped = true;
        GameObject equipGO = Instantiate(Resources.Load<GameObject>("prefabs/UI/EquippedCardIcon"), _equipped);
        EquippedCardIcon icon = equipGO.GetComponent<EquippedCardIcon>();
        icon.CardName = c.CardName;
        icon.CardObj = c.gameObject;
        _equippedCardIcons.Add(icon);
    }

    public void DeselectEquipped(EquippedCardIcon e)
    {
        Card c = e.CardObj.GetComponent<Card>();
        c.Equipped = false;
        _equippedCardIcons.Remove(e);
        Destroy(e.gameObject);
    }
    
    public void DeselectEquipped(Card c)
    {
        foreach (EquippedCardIcon e in _equippedCardIcons)
        {
            if (e.CardName == c.CardName)
            {
                c.Equipped = false;
                _equippedCardIcons.Remove(e);
                Destroy(e.gameObject);
                return;
            }
        }
    }

    public void CloseMenu()
    {
        if (_equippedCardIcons.Count < 9)
        {
            Utils.DisplayError("You must have at least 9 Cards!", 2f);
            return;
        }
        
        PlayerCollection pc = new PlayerCollection();
        
        foreach (Card c in _cardsOnScreen)
        {
            pc.Cards.Add(c.CardName);
        }

        foreach (EquippedCardIcon e in _equippedCardIcons)
        {
            pc.Equipped.Add(e.CardName);
        }
        
        Utils.Save(pc, "playercollection");
        gameObject.SetActive(false);
    }
}
