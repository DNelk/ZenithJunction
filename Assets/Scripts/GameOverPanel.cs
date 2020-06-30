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
    
    //animationSet
    private Animator _myAnim;
    private CanvasGroup mainCanvas;

    private Transform _transitionBattleEffect;

    private GameObject _magicParticle;

    // Start is called before the first frame update
    void Awake()
    {
        _result = transform.Find("ResultText").GetComponent<TMP_Text>();
        _cg = GetComponent<CanvasGroup>();
        _cg.alpha = 0;
        _buttons = transform.Find("Buttons").GetComponentsInChildren<Button>();
        
        _myAnim = GetComponent<Animator>();
        mainCanvas = BattleManager.Instance._mainCanvas.GetComponent<CanvasGroup>();
        //animation set init
        _transitionBattleEffect = transform.Find("TransitionBattleEffect");
        _magicParticle = _transitionBattleEffect.transform.Find("MagicParticle_End").gameObject;
            

    }

    private void Start()
    {
    }

    public void ShowResult(bool win)
    {
        if (win) _myAnim.SetTrigger("Win");
        else _myAnim.SetTrigger("Defeated");
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene("StartScreen");
    }

    public void Continue()
    {
        _myAnim.SetTrigger("Continue");
        NewCardChooser cardChooser = GameObject.Instantiate(Resources.Load<GameObject>("prefabs/CardChoiceDialog"), transform.parent).GetComponent<NewCardChooser>();

        //Unpack Map, if we win we move to enemy location and remove enemy. if we lose we reset our location
        MapData md = Utils.Load<MapData>("mapdata");
        if (Win)
        {
            cardChooser.SetTextSprite(true);
            md.PlayerNodeID = GameManager.Instance.BattlingNode;
            md.Enemies.RemoveAll(x => x.NodeID == GameManager.Instance.BattlingNode);
        }
        else cardChooser.SetTextSprite(false);
        
        Utils.Save(md, "mapdata");
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
    
    void turnOffCanvas()
    {
        //turn off main canvas
        mainCanvas.interactable = false;
        mainCanvas.DOFade(0, 0.5f);
        transform.parent.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
    
    void EndBattleEffectResize()
    {
        Image[] sprites = _transitionBattleEffect.GetComponentsInChildren<Image>();

        foreach (Image img in sprites)
        {
            img.SetNativeSize();
        }
    }

    void turnOnParticle()
    {
        _magicParticle.SetActive(true);
    }

    void turnOffButton()
    {
        foreach (Button b in _buttons)
        {
            Destroy(b.gameObject);
        }
    }
}  
