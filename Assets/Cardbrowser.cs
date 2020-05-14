using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cardbrowser : MonoBehaviour
{
    public GameObject[] Cards;

    private int i;
    
    private void Start()
    {
        Utils.Shuffle(Cards);
        foreach (var c in Cards)
        {
            c.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Cards[i].SetActive(false);
            i++;
            if (i > Cards.Length - 1)
                i = 0;
            Cards[i].SetActive(true);
        }
    }
}
