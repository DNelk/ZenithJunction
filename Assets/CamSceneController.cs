using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CamSceneController : MonoBehaviour
{
    public GameObject[] Cameras;
    public GameObject Sparks;
    public int CurrentCam  = 0;

    private void Start()
    {
        foreach (var c in Cameras)
        {
            c.SetActive(false);
        }
    }

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

        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("StartScreen");
    }

    private void NextCam()
    {
        if(CurrentCam >= 0)
            Cameras[CurrentCam].SetActive(false);
        CurrentCam++;
        if (CurrentCam == Cameras.Length)
            CurrentCam = -1;
        else
            Cameras[CurrentCam].SetActive(true);
    }
    
    private void PrevCam()
    {
        if(CurrentCam >= 0)
            Cameras[CurrentCam].SetActive(false);
        CurrentCam--;
        if (CurrentCam < -1)
            CurrentCam = Cameras.Length - 1;
        if (CurrentCam != -1)
            Cameras[CurrentCam].SetActive(true);
    }
}
