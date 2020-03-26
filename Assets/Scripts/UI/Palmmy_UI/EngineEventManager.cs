﻿using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class EngineEventManager : EventTrigger
{
    //engine ref
    private Engine _myEngine;

    private void Awake()
    {
        init();
    }

    void init()
    {
        _myEngine = GetComponent<Engine>(); //make ref to engine
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region pointer Enter&Exit
    
    public override void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(_myEngine._baseScale * 1.2f, 0.2f);
        _myEngine.playAuraAnim();
        _myEngine.selectGear();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(_myEngine._baseScale, 0.2f);
        _myEngine.disselectGear();
    }
    
    #endregion
}
