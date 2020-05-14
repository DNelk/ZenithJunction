using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConfirmButtonEventManager : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{

    private Animator _bannerAnim;

    void Awake()
    {
        _bannerAnim = GetComponentInChildren<Animator>();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (BattleManager.Instance.BattleState == BattleStates.MakingEngines)
            turnOn();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (BattleManager.Instance.BattleState == BattleStates.MakingEngines)
            turnOff();
    }

    public void turnOn()
    {
        _bannerAnim.SetBool("TurnOn", true);
    }

    public void turnOff()
    {
        _bannerAnim.SetBool("TurnOn", false);
    }

    public void setTrigger()
    {
        _bannerAnim.SetTrigger("GearTrigger");
    }

    public void reset()
    {
        _bannerAnim.Play("gear_Default");
    }
}
