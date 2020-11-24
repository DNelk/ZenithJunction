using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Arrow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image _myImage;

    //Awake initialize
    void Awake()
    {
        _myImage = GetComponent<Image>();
    }
    
    // call this everytime it become enable
    void OnEnable()
    {
        _myImage.color = new Color(1,1,1,0);
        _myImage.DOFade(1.0f, 0.5f);
    }

    //call this when mouse over
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = new Vector3(1,1,1);
        transform.DOScale(1.5f, 0.3f);
    }

    //call this when mouse exit
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = new Vector3(1.5f,1.5f,1.5f);
        transform.DOScale(new Vector3(1f,1f,1f), 0.3f);
    }
}
