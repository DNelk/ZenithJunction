using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class EngineCheatForRayCast : EventTrigger
{

    private Engine _myEngine;
    
    // Start is called before the first frame update
    void Start()
    {
        _myEngine = transform.parent.transform.Find("Engine1").GetComponent<Engine>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        BattleManager BM = BattleManager.Instance;
        
        if (!BM.isMouseDragging)
        {
            for (int i = 0; i < BM.Engines.Length; i++)
            {
                BM.Engines[i].highlightedOff();
                BM.Engines[i].disselectGear();

                BM.Engines[i].attackOnPositionPreviewOff();   
            }

            gameObject.SetActive(false);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        /*if (!BattleManager.Instance.isMouseDragging)
        {
            _myEngine.transform.DOScale(_myEngine._baseScale, 0.2f);
            _myEngine.disselectGear();

            _myEngine.attackOnPositionPreviewOff();
            
            gameObject.SetActive(false);
        }*/
    }
}
