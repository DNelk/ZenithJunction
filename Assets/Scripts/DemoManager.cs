using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DemoManager : MonoBehaviour
{
    public void LoadBeginner()
    {
        SceneManager.LoadScene("Scenes/Overworld");
    }
    
    public void LoadElite()
    {
        SceneManager.LoadScene("Scenes/SlimeBattle");
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
            LoadBeginner();
        if(Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("CameraScene");
    }
}
