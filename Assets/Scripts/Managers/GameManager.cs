using System;
using System.Collections;
using System.Collections.Generic;
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
        else if(Instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
        
        if(CardDirectory.CardsByName.Count == 0)
            CardDirectory.LoadDirectory();
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
