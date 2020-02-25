using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; //Required when using Event data.
using UnityEngine.UI;

public class DummyIntention : MonoBehaviour
    , IPointerEnterHandler //required interface when using the OnPointerEnter method.
    , IPointerExitHandler
    , IPointerClickHandler
{

    private Vector3 _baseScale;
    private Image _myImage;
    private Animator _intWindow;
    private bool isHover;
    
    // Start is called before the first frame update
    void Start()
    {
        _baseScale = transform.localScale;
        _myImage = GetComponent<Image>();
        _myImage.color =  new Color(0.8f,0.8f,0.8f,1);

        _intWindow = transform.parent.transform.parent.transform.Find("DataWindow").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isHover)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _intWindow.SetBool("IsON",false);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = _baseScale * 1.5f;
        _myImage.color =  new Color(1,1,1,1);
        isHover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = _baseScale;
        _myImage.color =  new Color(0.8f,0.8f,0.8f,1);
        isHover = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _intWindow.SetBool("IsON",!_intWindow.GetBool("IsON"));
    }
}
