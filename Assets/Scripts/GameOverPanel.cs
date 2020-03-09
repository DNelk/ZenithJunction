using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    private TMP_Text _result;
    public bool Win;
    private CanvasGroup _cg;
    private Button[] _buttons;
    // Start is called before the first frame update
    void Awake()
    {
        _result = transform.Find("ResultText").GetComponent<TMP_Text>();
        _cg = GetComponent<CanvasGroup>();
        _cg.alpha = 0;
        _buttons = transform.Find("Buttons").GetComponentsInChildren<Button>();
    }

    private void Start()
    {
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene("StartScreen");
    }

    public void Continue()
    {
        Instantiate(Resources.Load<GameObject>("prefabs/CardChoiceDialog"), transform.parent);
        
        //Unpack Map, if we win we move to enemy location and remove enemy. if we lose we reset our location
        MapData md = Utils.Load<MapData>("mapdata");
        if (Win)
        {
            md.PlayerNodeID = GameManager.Instance.BattlingNode;
            md.Enemies.RemoveAll(x => x.NodeID == GameManager.Instance.BattlingNode);
        }
        
        Utils.Save(md, "mapdata");

        Destroy(gameObject);
    }
    
    public string Result
    {
        get => _result.text;
        set => _result.text = value;
    }

    public void Load(string msg, bool win)
    {
        Result = msg;
        Win = win;
        _cg.DOFade(1, 0.2f);
    }
}  
