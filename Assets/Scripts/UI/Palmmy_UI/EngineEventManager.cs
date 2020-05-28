using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class EngineEventManager : EventTrigger
{
    //engine ref
    private Engine _myEngine;
    
    //animationcheck
    private bool _inAnim;

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
        if (!BattleManager.Instance.isMouseDragging)
        {
            _myEngine.highlightedOn();
            _myEngine.playAuraAnim();
            _myEngine.selectGear();
            _inAnim = true;

            _myEngine.attackOnPositionPreviewOn();

            Engine[] BMEngines = BattleManager.Instance.Engines;
            
            for (int i = 0; i < BMEngines.Length ; i++)
            {
                if (BMEngines[i] != _myEngine && !BMEngines[i]._selected)
                {
                    BMEngines[i].transform.DOScale(_myEngine._baseScale, 0.2f);
                    BMEngines[i].disselectGear();
                    BMEngines[i].attackOnPositionPreviewOff();
                }
            }
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!BattleManager.Instance.isMouseDragging)
        {
            if (!_myEngine._selected)
            {
                _myEngine.highlightedOff();
                _myEngine.disselectGear();
            }
        
            _myEngine.attackOnPositionPreviewOff();
        }
    }
    
    #endregion
}
