using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DemoManager : MonoBehaviour
{
    public GameObject loopMap;
    public GameObject currentMap;
    public void LoadBeginner()
    {
        SceneManager.LoadScene("Scenes/Overworld");
    }
    
    public void LoadElite()
    {
        SceneManager.LoadScene("Scenes/SlimeBattle");
    }

    public static DemoManager Instance;
    
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.Return))
            LoadBeginner();
        if(Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("CameraScene");*/
    }

    public void LoopDemo()
    {
        //Clear the current map
        Destroy(currentMap);
        //Spawn a fresh loop map
        GameObject newMap = Instantiate(loopMap);
        newMap.transform.position = Vector3.zero;
        //Save the fresh map
        MapManager.Instance.SaveMap();
        //Reload the scene
        SceneManager.LoadScene("Overworld");
    }
}
