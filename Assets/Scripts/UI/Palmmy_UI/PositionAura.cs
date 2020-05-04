using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PositionAura : MonoBehaviour
{
    private Animator myAnim;
    private Image myAura;

    private bool isFading;
    
    // Start is called before the first frame update
    void Start()
    {
        myAnim = GetComponent<Animator>();
        myAura = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFading)
        {
            if (myAura.color.a > 0)
                myAura.color -= new Color(0,0,0,0.002f);
            else if (myAura.color.a <= 0)
                isFading = false;
        }
    }

    public void animate()
    {
        myAnim.SetBool("isAnimate", true);
    }

    public void stopAnimate()
    {
        myAnim.SetBool("isAnimate", false);
        isFading = true;
    }
}
