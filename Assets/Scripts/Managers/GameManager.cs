using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState State;

    //Map Stuff
    private string _currentEnagingInteractable;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        
        if(CardDirectory.CardsByName.Count == 0)
            CardDirectory.LoadDirectory();
        if (!File.Exists(Application.persistentDataPath + "playercollection.save"))
        {
            PlayerCollection pc = new PlayerCollection();
            pc.Cards.Add("strike");
            pc.Cards.Add("strike");
            pc.Cards.Add("strike");
            pc.Cards.Add("strike");
            pc.Cards.Add("railcharge");
            pc.Cards.Add("allaboard");
            pc.Cards.Add("manaboil");
            pc.Cards.Add("manaboil");
            pc.Cards.Add("manaboil");
            //pc.Cards.Add("DevRage");
            
            pc.Equipped.Add("strike");
            pc.Equipped.Add("strike");
            pc.Equipped.Add("strike");
            pc.Equipped.Add("strike");
            pc.Equipped.Add("railcharge");
            pc.Equipped.Add("allaboard");
            pc.Equipped.Add("manaboil");
            pc.Equipped.Add("manaboil");
            pc.Equipped.Add("manaboil");
            
            Utils.Save(pc, "playercollection");
        }
    }

    private void Start()
    {
        
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    
}

public enum GameState
{
    Battle,
    WorldMap,
    Paused,
    Customizing,
    Acquiring
}
