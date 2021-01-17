using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;




public class GameManager : MonoBehaviour, ISerializationCallbackReceiver
{
    public static GameManager Instance;
    public GameState State;

    //Map Stuff
    public string BattlingNode;

    //Player save
    [HideInInspector] public List<string> StartingCards = new List<string>(){"strike", "strike", "strike", "manaboil", "manaboil", "manaboil", "strike", "allaboard", "railcharge"};
    [HideInInspector] [SerializeField] public int[] StartingIndices;
    public static int[] StaticIndices = {51,51,51,51,40,1,31,31,31};
    
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
        if (!File.Exists(Application.persistentDataPath + "/playercollection.save") || PlayerPrefs.GetInt("restart") == 1)
        {
            ResetPlayerSave();
        }
    }

    private void Start()
    {
        Debug.Log(Application.persistentDataPath);
    }

    private int capnum = 0;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("capture get");
            capnum++;
            ScreenCapture.CaptureScreenshot(Application.persistentDataPath+"/Screencap"+capnum+".png");
        }
            
    }

    public void StartBattle()
    {
        BattleManager.Instance.InitializeBattle();

        if (TutorialManager.Instance != null)
            TutorialManager.Instance.Step();
        else
            DeckManager.Instance.DealHand();
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        GameObject loading = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Loading"), GameObject.Find("MainCanvas").transform);
        
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        Destroy(loading);
    }

    public void ResetPlayerSave()
    {
        PlayerCollection pc = new PlayerCollection();
        foreach (var c in StartingCards)
        {
           pc.Cards.Add(c);
        }

        for (int i = 0; i < 9; i++)
        {
            pc.Equipped.Add(StartingCards[i]);
        }
        Utils.Save(pc, "playercollection");
    }
    
    public void ResetPlayerSaveHard()
    {
        List<string> startingCards = new List<string>(){"strike", "strike", "strike", "manaboil", "manaboil", "manaboil", "strike", "allaboard", "railcharge"};

        PlayerCollection pc = new PlayerCollection();
        foreach (var c in startingCards)
        {
            pc.Cards.Add(c);
        }

        for (int i = 0; i < 9; i++)
        {
            pc.Equipped.Add(startingCards[i]);
        }
        Utils.Save(pc, "playercollection");
    }

    public void OnAfterDeserialize()
    {
        StartingIndices = StaticIndices;
    }

    public void OnBeforeSerialize()
    {
        StaticIndices = StartingIndices;
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
