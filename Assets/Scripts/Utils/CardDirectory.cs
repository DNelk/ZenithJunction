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

    public static void SaveDirectory()
    {
        CardDirectoryData data = new CardDirectoryData();
        
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
            data.CardsByName.Add(new CardEntry(result[0].ToLower(), name));
            
            //if this is a basic or debug card it doesnt have archetype or rarity... moving on!
            if(result.Length == 1)
                continue;
            //Separate by archetype and rarity
            char archetype = result[1][0];
            char rarity = result[1][1];
            
            switch (archetype)
            {
                case 'A':
                    data.BigEcon.Add(new CardEntry(name, rarity.ToString()));
                    break;
                case 'R':
                    data.Ranged.Add(new CardEntry(name, rarity.ToString()));
                    break;
                case 'M':
                    data.Melee.Add(new CardEntry(name, rarity.ToString()));
                    break;
                default:
                    break;
            }
        }
        
        Utils.SaveStreaming(data, "CardDirectory");
    }
    public static void LoadDirectory()
    {
        CardDirectoryData data = Utils.LoadStreaming<CardDirectoryData>("CardDirectory");

        foreach (var b in data.CardsByName)
        {
            CardsByName.Add(b.Name, b.Value);
        }

        foreach (var a in data.BigEcon)
        {
            char rarity = a.Value[0];
            switch (rarity)
            {
                case 'C':
                    BigEcon.Add(a.Name, CardRarities.Common);
                    break;
                case 'U':
                    BigEcon.Add(a.Name, CardRarities.Uncommon);
                    break;
                case 'R':
                    BigEcon.Add(a.Name, CardRarities.Rare);
                    break;
                case 'M':
                    BigEcon.Add(a.Name, CardRarities.UltraRare);
                    break;
                default:
                    break;
            }
        }
        
        foreach (var m in data.Melee)
        {
            char rarity = m.Value[0];
            switch (rarity)
            {
                case 'C':
                    Melee.Add(m.Name, CardRarities.Common);
                    break;
                case 'U':
                    Melee.Add(m.Name, CardRarities.Uncommon);
                    break;
                case 'R':
                    Melee.Add(m.Name, CardRarities.Rare);
                    break;
                case 'M':
                    Melee.Add(m.Name, CardRarities.UltraRare);
                    break;
                default:
                    break;
            }
        }
        
        foreach (var r in data.Ranged)
        {
            char rarity = r.Value[0];
            switch (rarity)
            {
                case 'C':
                    Ranged.Add(r.Name, CardRarities.Common);
                    break;
                case 'U':
                    Ranged.Add(r.Name, CardRarities.Uncommon);
                    break;
                case 'R':
                    Ranged.Add(r.Name, CardRarities.Rare);
                    break;
                case 'M':
                    Ranged.Add(r.Name, CardRarities.UltraRare);
                    break;
                default:
                    break;
            }
        }

        //Debug.Log("Card Directory Loaded");
    }

    public static string GetRandomCard(CardArchetype archetype, CardRarities rarity, bool split = false)
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
            return GetRandomCard(archetype, CardRarities.Common, split);
        
        //Now get random from rarity list
        string rndC =  rarityList[Random.Range(0, rarityList.Count - 1)];

        if (split)
            return rndC.Split('_')[0];
        return rndC;
    }
    
    
}