using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class CamSceneController : MonoBehaviour
{
    public GameObject[] Cameras;
    public GameObject Sparks;
    public int CurrentCam  = 0;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            NextCam();
        if (Input.GetKeyDown(KeyCode.Backspace))
            PrevCam();
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (Sparks.activeSelf)
                Sparks.SetActive(false);
            else
                Sparks.SetActive(true);
        }
    }

    private void NextCam()
    {
        Cameras[CurrentCam].SetActive(false);
        CurrentCam++;
        if (CurrentCam > Cameras.Length - 1)
            CurrentCam = 0;
        Cameras[CurrentCam].SetActive(true);
    }
    
    private void PrevCam()
    {
        Cameras[CurrentCam].SetActive(false);
        CurrentCam--;
        if (CurrentCam < 0)
            CurrentCam = Cameras.Length-1;
        Cameras[CurrentCam].SetActive(true);
    }
}
