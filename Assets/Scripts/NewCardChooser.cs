using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class NewCardChooser : MonoBehaviour
{
    public static NewCardChooser Instance;

    private Transform _cardPanel;

    public bool CardChosen;
    
    public Sprite[] chooseCardSprite = new Sprite[2];
    public Sprite[] continueSprite = new Sprite[2];
    private Image[] textImage = new Image[2];
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if(Instance != this)
            Destroy(gameObject);
        
        //Init
        _cardPanel = transform.Find("CardPanel").transform;
        CardChosen = false;
        GameManager.Instance.State = GameState.Acquiring;

        textImage[0] = transform.Find("ChooseNewCard_Text").GetComponent<Image>();
        textImage[1] = transform.Find("Buttons").transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
    }

    private void Start()
    {
        Card[] cards = new Card[3];
        cards[0] = Instantiate(Utils.LoadCard(CardDirectory.GetRandomCard(CardArchetype.Melee, Utils.GetRandomRarity()), true), _cardPanel).GetComponent<Card>();
        cards[1] = Instantiate(Utils.LoadCard(CardDirectory.GetRandomCard(CardArchetype.BigEcon, Utils.GetRandomRarity()), true), _cardPanel).GetComponent<Card>();
        cards[2] = Instantiate(Utils.LoadCard(CardDirectory.GetRandomCard(CardArchetype.Ranged, Utils.GetRandomRarity()), true), _cardPanel).GetComponent<Card>();
        foreach (var c in cards)
        {
            c.ShowFullSize = true;
        }
    }

    public void SetTextSprite(bool Win)
    {
        int numberOfSprite = 0;
        if (Win) numberOfSprite = 0;
        else numberOfSprite = 1;
        
        textImage[0].sprite = chooseCardSprite[numberOfSprite];
        textImage[0].SetNativeSize();
        textImage[1].sprite = continueSprite[numberOfSprite];
        textImage[1].SetNativeSize();

    }

    public void ChooseCard(string name)
    {
        CardChosen = true;
        PlayerCollection collection = Utils.Load<PlayerCollection>("playercollection");
        collection.Cards.Add(name);
        Utils.Save(collection, "playercollection");
    }

    public void ToMap()
    {
        SceneManager.LoadScene("Overworld");
    }
}
