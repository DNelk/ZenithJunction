using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Map_Hovering : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image _mySpriteRenderer;
    public Animator auraAnim;

    public Sprite[] _mySpr;
    
    // Start is called before the first frame update
    void Start()
    {
        _mySpriteRenderer = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _mySpriteRenderer.sprite = _mySpr[1];
        _mySpriteRenderer.SetNativeSize();

        if (auraAnim != null)
        {
            if (!auraAnim.GetBool("TurnOn"))
                auraAnim.SetBool("TurnOn", true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _mySpriteRenderer.sprite = _mySpr[0];
        _mySpriteRenderer.SetNativeSize();
        
        if (auraAnim != null)
        {
            if (auraAnim.GetBool("TurnOn"))
                auraAnim.SetBool("TurnOn", false);
        }
    }
}
