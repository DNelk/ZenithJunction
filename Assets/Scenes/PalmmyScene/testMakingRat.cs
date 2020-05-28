using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class testMakingRat : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public GameObject Image;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Image.SetActive(true);
            Debug.Log("Hmmm");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Image.SetActive(false);
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("in");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Off");
    }
}
