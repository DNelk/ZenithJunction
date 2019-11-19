using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyIntentionUI : MonoBehaviour
{
    private TMP_Text _myText;

    private void Awake()
    {
        _myText = transform.Find("Back").transform.Find("Text").GetComponent<TMP_Text>();
    }

    public string Text
    {
        get => _myText.text;
        set => _myText.text = value;
    }
}
