using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Random = UnityEngine.Random;

public static class CardDirectory
{

    //A dictionary with all cards by name
    public static Dictionary<string, string> CardsByName = new Dictionary<string, string>();

    //Cards by Archetype and Rarity
    public static Dictionary<string, CardRarities> BigEcon = new Dictionary<string, CardRarities>();
    public static Dictionary<string, CardRarities> Ranged = new Dictionary<string, CardRarities>();
    public static Dictionary<string, CardRarities> Melee = new Dictionary<string, CardRarities>();

    public static void LoadDirectory()
    {
        DirectoryInfo dir = new DirectoryInfo("Assets/Resources/Prefabs/Cards");
        FileInfo[] info = dir.GetFiles("*.*");
        foreach (FileInfo f in info)
        {
            //No meta files
            if(f.Name.Contains(".meta"))
                continue;
            string name = f.Name.Split('.')[0];
            //Debug.Log(name);
            //Break up the card into cardname and archetype and rarity
            string[] result = name.Split('_');
            
            //Add the card by name
            CardsByName.Add(result[0].ToLower(), name);
            
            //if this is a basic or debug card it doesnt have archetype or rarity... moving on!
            if(result.Length == 1)
                continue;
            //Separate by archetype and rarity
            char archetype = result[1][0];
            char rarity = result[1][1];

            CardRarities myRarity = CardRarities.Common;
            switch (rarity)
            {
                case 'C':
                    myRarity = CardRarities.Common;
                    break;
                case 'U':
                    myRarity = CardRarities.Uncommon;
                    break;
                case 'R':
                    myRarity = CardRarities.Rare;
                    break;
                case 'M':
                    myRarity = CardRarities.UltraRare;
                    break;
                default:
                    break;
            }
            
            switch (archetype)
            {
                case 'A':
                    BigEcon.Add(name, myRarity);
                    break;
                case 'R':
                    Ranged.Add(name, myRarity);
                    break;
                case 'M':
                    Melee.Add(name, myRarity);
                    break;
                default:
                    break;
            }
        }
        
        //Debug.Log("Card Directory Loaded");
    }

    public static string GetRandomCard(CardArchetype archetype, CardRarities rarity)
    {
        //Search the right list
        Dictionary<string, CardRarities> currentList;
        
        switch (archetype)
        {
            case CardArchetype.BigEcon:
                currentList = BigEcon;
                break;
            case CardArchetype.Melee:
                currentList = Melee;
                break;
            case CardArchetype.Ranged:
                currentList = Ranged;
                break;
            default:
                return "error";
        }

        //Now sort by rarity
        List<string> rarityList = new List<string>();

        foreach (var c in currentList)
        {
            if (c.Value == rarity)
                rarityList.Add(c.Key);
        }

        if (rarityList.Count == 0)
            return GetRandomCard(archetype, CardRarities.Common);
        
        //Now get random from rarity list
        return rarityList[Random.Range(0, rarityList.Count - 1)];
    }
}