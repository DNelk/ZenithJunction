using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static void DisplayError(string err, float time)
    {
        ErrorMessage error = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Error"), GameObject.Find("Canvas").transform).GetComponent<ErrorMessage>();
        error.StartCoroutine(error.StartFade(err, time));
    }
}
