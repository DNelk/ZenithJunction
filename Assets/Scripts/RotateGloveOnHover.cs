using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGloveOnHover : MonoBehaviour
{
    private Animator _anim;

    private void Awake()
    {
        _anim = transform.parent.GetChild(1).GetChild(0).GetComponent<Animator>();
    }

    public void StartRotate()
    {
        _anim.SetBool("Hovering", true);
    }
    
    public void StopRotate()
    {
        _anim.SetBool("Hovering", false);
    }
}
