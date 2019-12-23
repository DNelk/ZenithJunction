using System;
using System.Collections;
using System.Collections.Generic;
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
        if (BattleManager.Instance.BattleState == BattleStates.BuyingCards)
        {
            parent = BuyManager.Instance.transform;
        }
        else
        {
            parent = GameObject.Find("MainCanvas").transform;
        }
        CurrentPreview = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/CardPreview"), parent).GetComponent<CardPreview>();
        if (BattleManager.Instance.BattleState == BattleStates.BuyingCards)
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
}
