using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
}
