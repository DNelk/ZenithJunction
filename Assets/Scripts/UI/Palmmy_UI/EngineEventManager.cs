using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class EngineEventManager : EventTrigger
{
    //engine
    private Engine _myEngine;

    private void Awake()
    {
        init();
    }

    void init()
    {
        _myEngine = GetComponent<Engine>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(_myEngine._baseScale * 1.2f, 0.2f);
        _myEngine.playAuraAnim();
        _myEngine.turnGearOn();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(_myEngine._baseScale, 0.2f);
        _myEngine.turnGearOff();
    }
}
