using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public static class Utils
{
    public static void DisplayError(string err, float time)
    {
        ErrorMessage error = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Error"), GameObject.Find("MainCanvas").transform).GetComponent<ErrorMessage>();
        error.StartCoroutine(error.StartFade(err, time));
    }

    public static void DisplayGameOver(string message)
    {
        BattleManager.Instance.BattleState = BattleStates.GameOver;
        GameOverPanel go = GameObject.Instantiate(Resources.Load<GameObject>("prefabs/GameOver"), GameObject.Find("MainCanvas").transform).GetComponent<GameOverPanel>();
        go.Load(message);
    }

    public static CardPreview CurrentPreview;
    public static void GenerateCardPreview(Card c)
    {
        if(CurrentPreview != null)
            return;
        Transform parent = null;
        if (BattleManager.Instance != null && BattleManager.Instance.BattleState == BattleStates.BuyingCards)
        {
            parent = BuyManager.Instance.transform;
        }
        else
        {
            parent = GameObject.Find("MainCanvas").transform;
        }
        CurrentPreview = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/CardPreview"), parent).GetComponent<CardPreview>();
        if (BattleManager.Instance != null && BattleManager.Instance.BattleState == BattleStates.BuyingCards)
        {
            CurrentPreview.transform.GetChild(0).transform.localScale = Vector3.one * 1.8f;
        }
        CurrentPreview.SetCard(c);
    }

    public static void DestroyCardPreview()
    {
        if(CurrentPreview == null)
            return;
        CurrentPreview.Destroy();

    }

    public static bool FlipCoin()
    {
        int rng = UnityEngine.Random.Range(0, 2);
        return rng == 0;
    }

    public static string ReplaceWithSymbols(string org)
    {
        string txtWSymbols = org;
        
        //Replace power
        txtWSymbols = txtWSymbols.Replace("Power", "power");
        txtWSymbols = txtWSymbols.Replace("power", " <sprite=\"icons\" name=\"icons_power\"> ");

        //Replace aether
        txtWSymbols = txtWSymbols.Replace("Aether", "aether");
        txtWSymbols = txtWSymbols.Replace("aether", " <sprite=\"icons\" name=\"icons_aether\"> ");

        //Replace move
        txtWSymbols = txtWSymbols.Replace("Move", "move");
        txtWSymbols = txtWSymbols.Replace("move", " <sprite=\"icons\" name=\"icons_move\"> ");

        return txtWSymbols;
    }

    public static string[] Shuffle(string[] toShuffle)
    {
        Random rng = new Random();
        int i = toShuffle.Length;
        while (i > 1)
        {
            i--;
            int k = rng.Next(i + 1);
            String tempC = toShuffle[k];
            toShuffle[k] = toShuffle[i];
            toShuffle[i] = tempC;
        }

        return toShuffle;
    }

    public static void Save<T>(T dataToSave, string filename)
    {
        filename += ".save";
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + filename);
        bf.Serialize(file, dataToSave);
        file.Close();

       // Debug.Log(filename + " Saved");
    }

    public static T Load<T>(string filename)
    {
        filename += ".save";
        if (File.Exists(Application.persistentDataPath + filename))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + filename, FileMode.Open);
            T save = (T) bf.Deserialize(file);
            file.Close();
            
            //Debug.Log(filename + " Loaded");
            return save;
        }
        
        Debug.Log(filename + ": file not found!");
        return default(T);
    }

    public static GameObject LoadCard(string cardname, bool fullCardName = false)
    {
        if (fullCardName)
            return Resources.Load<GameObject>("prefabs/cards/" + cardname);
        string key = cardname.Replace(" ", String.Empty).ToLower();
        return Resources.Load<GameObject>("prefabs/cards/" + CardDirectory.CardsByName[key]);
    }
    
    //TODO:Change this! Rarity odds should shift with player collection
    public static CardRarities GetRandomRarity()
    {
        int rnd = UnityEngine.Random.Range(0, 100);

        if (rnd <= 30)
            return CardRarities.Uncommon;
        if (rnd <= 45)
            return CardRarities.Rare;
        if (rnd <= 48)
            return CardRarities.UltraRare;
        return CardRarities.Common;
    }
    
    public static CardArchetype GetRandomArchetype()
    {
        int rnd = UnityEngine.Random.Range(0, 3);

        switch (rnd)
        {
            case 0:
                return CardArchetype.Melee;
            case 1:
                return CardArchetype.Ranged;
            case 2:
                return CardArchetype.BigEcon;
        }

        return CardArchetype.Melee;
    }

    public static bool CompareCardName(string a, string b)
    {
        if (a.ToLower().Replace(" ", string.Empty) == b.ToLower().Replace(" ", string.Empty))
            return true;
        return false;
    }
}
