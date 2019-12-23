using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DemoManager : MonoBehaviour
{
    public void LoadBeginner()
    {
        SceneManager.LoadScene("Scenes/PrototypeBattle");
    }
    
    public void LoadElite()
    {
        SceneManager.LoadScene("Scenes/SlimeBattle");
    }
}
