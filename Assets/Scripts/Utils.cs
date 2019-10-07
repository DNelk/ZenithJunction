using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public static class Utils
{
    public static void DisplayError(string err, float time)
    {
        ErrorMessage error = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Error"), GameObject.Find("Canvas").transform).GetComponent<ErrorMessage>();
        error.StartCoroutine(error.StartFade(err, time));
    }

    public static void DisplayGameOver(string message)
    {
        PlayerPrefs.SetString("GameOverMessage", message);
        SceneManager.LoadScene("GameOver");
    }

    public static CardPreview CurrentPreview;
    public static void GenerateCardPreview(Card c)
    {
        if(CurrentPreview != null)
            return;
        CurrentPreview = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/CardPreview"), GameObject.Find("Canvas").transform).GetComponent<CardPreview>();
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
        int rng = Random.Range(0, 2);
        return rng == 0;
    }
}
