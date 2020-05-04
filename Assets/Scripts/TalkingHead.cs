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
    private Animator _anim;
    
    public Image BG;

    public bool IsIdle = false;
    public bool TextDone = true;
    public float TextTimer;

    private float _textSpeed = 0.02f;

    public Sprite[] Portraits;
    public Sprite Sprite
    {
        get => _characterSprite.sprite;
        set => _characterSprite.sprite = value;
    }

    public string CharacterName
    {
        get => _characterName.text;
        set
        {
            _characterName.text = value;
            switch (_characterName.text)
            {
                case "Hugo":
                    Sprite = Portraits[0];
                    break;
                case "Sir John":
                    Sprite = Portraits[1];
                    break;
                case "Sir Wolff":
                    Sprite = Portraits[2];
                    break;
                default:
                    Sprite = null;
                    break;
            }
        }
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
        _anim = GetComponent<Animator>();
    }
    
    public void SetIdle()
    {
        IsIdle = true;
    }
    
    //Print dialogue one char at a time
    private IEnumerator PrintDialogue()
    {
        TextTimer = 0f;
        string currentText = Dialogue;
        bool tagOpen = false;
        for (int i = 0; i < currentText.Length; i++)
        {
            string subStr = currentText.Substring(0, i);
            
            //Check tags
            if (tagOpen && currentText[i] == '>')
            {
                tagOpen = false;
                continue;
            }

            if (currentText[i] == '<')
                tagOpen = true;
            if(tagOpen)
                continue;
            
            //Set text and wait
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

    public void RollOut()
    {
        _anim.SetTrigger("RollOut");
    }

    public void SetOutOfFrame()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        if (TextDone)
            TextTimer += Time.deltaTime;
    }
}
