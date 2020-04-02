using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TalkingHead : MonoBehaviour
{
    private Image _characterSprite;
    private TMP_Text _characterName;
    private TMP_Text _dialogue;

    public Sprite Sprite
    {
        get => _characterSprite.sprite;
        set => _characterSprite.sprite = value;
    }

    public string Name
    {
        get => _characterName.text;
        set => _characterName.text = value;
    }
    
    public string Dialogue
    {
        get => _dialogue.text;
        set => _dialogue.text = value;
    }
    private void Awake()
    {
        _characterSprite = transform.Find("Character").GetComponent<Image>();
        _characterName = transform.Find("Name").GetComponentInChildren<TMP_Text>();
        _dialogue = transform.Find("DialogueBox").GetComponentInChildren<TMP_Text>();
    }
    
    
}
