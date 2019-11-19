using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPopIn : MonoBehaviour
{
    private TMP_Text _myText;
    private Animator _anim;
    private Vector3 _defaultPos;
    private Image _myImage;
    private CanvasGroup _cg;

    private bool _animComplete = false;
    private void Awake()
    {
        _myText = GetComponentInChildren<TMP_Text>();
        _anim = GetComponent<Animator>();
        _myImage = GetComponent<Image>();
        _cg = GetComponent<CanvasGroup>();
    }

    public void ToggleAnimComplete()
    {
        _animComplete = !_animComplete;
    }

    public void PopIn()
    {
        _anim.SetTrigger("PopIn");
    }

    public void FadeOut()
    {
        _anim.SetTrigger("FadeOut");
    }

    public void ResetPos()
    {
        _animComplete = false;
        Color = Color.white;
    }

    public float Alpha
    {
        get => _cg.alpha;
        set => _cg.alpha = 0;
    }
    
    public string Text
    {
        get => _myText.text;
        set => _myText.text = value;
    }
    
    public bool AnimationComplete
    {
        get => _animComplete;
    }

    public Color Color
    {
        set => _myText.color = value;
    }
}
