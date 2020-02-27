using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; //Required when using Event data.
using UnityEngine.UI;



public class BattleUI_Hover : MonoBehaviour
    , IPointerEnterHandler //required interface when using the OnPointerEnter method.
    , IPointerExitHandler //required interface when using the OnPointerExit method.

{
    public Image[] MainPad_Glow;
    public Animator[] MainPad_Light;

    public Animator[] gear;

    public Animator[] BG;
    public Image[] subBG;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < BG.Length; i++)
        {
            BG[i].GetComponent<Image>().color = new Color(1, 1, 1, 0.8f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //turn on MainPad_Glow
        for (int i = 0; i < MainPad_Glow.Length; i++)
        {
            MainPad_Glow[i].color = new Color(1,1,1,1);
        }

        //turn on MainPadLight
        for (int i = 0; i < MainPad_Light.Length; i++)
        {
            if (MainPad_Light[i].GetBool("LightOn") == false)
                MainPad_Light[i].SetBool("LightOn", true);
        }

        //turn BlackMask on
        for (int i = 0; i < BG.Length; i++)
        {
            BG[i].transform.position = transform.position;
            if (BG[i].GetBool("TurnOn") == false) 
                BG[i].SetBool("TurnOn", true);
        }
        
        //turn on Gear
        for (int i = 0; i < gear.Length; i++)
        {
            if (gear[i].GetBool("TurnOn") == false) 
                gear[i].SetBool("TurnOn", true);
        }
        
        //turn BlackMask2 on (If have one)
        for (int i = 0; i < subBG.Length; i++)
        {
            subBG[i].color = new Color(0,0,0,0.6f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //turn 0ff MainPad_Glow
        for (int i = 0; i < MainPad_Glow.Length; i++)
        {
            MainPad_Glow[i].color = new Color(1,1,1,0);
        }
        
        //turn off MainPadLight
        for (int i = 0; i < MainPad_Light.Length; i++)
        {
            if (MainPad_Light[i].GetBool("LightOn") == true)
                MainPad_Light[i].SetBool("LightOn", false);
        }

        //turn BlackMask off
        for (int i = 0; i < BG.Length; i++)
        {
            if (BG[i].GetBool("TurnOn") == true) 
                BG[i].SetBool("TurnOn",  false);
        }
        
        //turn off Gear
        for (int i = 0; i < gear.Length; i++)
        {
            if (gear[i].GetBool("TurnOn") == true) 
                gear[i].SetBool("TurnOn", false);
        }
        
        //turn BlackMask2 off (If have one)
        for (int i = 0; i < subBG.Length; i++)
        {
            subBG[i].color = new Color(1,1,1,0);
        }
    }
}
