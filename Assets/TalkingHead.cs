using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TalkingHead : MonoBehaviour
{
    private Image _characterSprite;
    private TMP_Text _characterName;
    private TMP_Text _dialogue;
    public Image BG;

    public bool IsIdle = false;
    public bool TextDone = true;

    private float _textSpeed = 0.01f;
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
        set
        {
            _dialogue.text = value;
            TextDone = false;
            StartCoroutine(PrintDialogue());
        }
    }
    private void Awake()
    {
        _characterSprite = transform.Find("Character").GetComponent<Image>();
        _characterName = transform.Find("Name").GetComponentInChildren<TMP_Text>();
        _dialogue = transform.Find("DialogueBox").GetComponentInChildren<TMP_Text>();
        _dialogue.text = "";
        BG = GetComponent<Image>();
    }
    
    public void SetIdle()
    {
        IsIdle = true;
    }
    
    private IEnumerator PrintDialogue()
    {
        string currentText = Dialogue;
        for (int i = 0; i < currentText.Length; i++)
        {
            string subStr = currentText.Substring(0, i);
            _dialogue.text = subStr;
            yield return new WaitForSeconds(_textSpeed);
        }

        _dialogue.text = currentText;
        TextDone = true;
    }

    public void FadeInBG()
    {
        BG.DOFade(0.8f, 1f);
    }
    
}
