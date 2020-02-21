using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class NewCardChooser : MonoBehaviour
{
    public static NewCardChooser Instance;

    private Transform _cardPanel;

    public bool CardChosen;
    
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
    }

    private void Start()
    {
        Card[] cards = new Card[3];
        cards[0] = Instantiate(Utils.LoadCard(CardDirectory.GetRandomCard(CardArchetype.Melee, GetRandomRarity()), true), _cardPanel).GetComponent<Card>();
        cards[1] = Instantiate(Utils.LoadCard(CardDirectory.GetRandomCard(CardArchetype.BigEcon, GetRandomRarity()), true), _cardPanel).GetComponent<Card>();
        cards[2] = Instantiate(Utils.LoadCard(CardDirectory.GetRandomCard(CardArchetype.Ranged, GetRandomRarity()), true), _cardPanel).GetComponent<Card>();
        foreach (var c in cards)
        {
            c.ShowFullSize = true;
        }
    }

    //Change this! Rarity odds should shift with player collection
    private CardRarities GetRandomRarity()
    {
        int rnd = Random.Range(0, 100);

        if (rnd <= 30)
            return CardRarities.Uncommon;
        if (rnd <= 45)
            return CardRarities.Rare;
        if (rnd <= 48)
            return CardRarities.UltraRare;
        return CardRarities.Common;
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
