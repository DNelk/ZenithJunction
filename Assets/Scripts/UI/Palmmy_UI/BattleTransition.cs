using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BattleTransition : MonoBehaviour
{
    private Animator _myAnim;
    private Enemy boss;
    private CanvasGroup mainCanvas;
    
    private Transform _transitionBattleInfo;
    private Transform _transitionBattleEffect;

    private GameObject[] _magicParticle = new GameObject[2];

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Init()
    {
        _myAnim = GetComponent<Animator>();
        boss = BattleManager.Instance.CurrentEnemy;

        _transitionBattleInfo = transform.Find("BeginBattleInfo");
        _transitionBattleEffect = transform.Find("TransitionBattleEffect");

        _magicParticle[0] = _transitionBattleEffect.transform.Find("MagicParticle_Begin").gameObject;
        _magicParticle[1] = _transitionBattleEffect.transform.Find("MagicParticle_End").gameObject;
    }

    // use update for debug only
    void Update()
    {
        //for debug only
        //if (Input.GetKeyDown(KeyCode.A)) _myAnim.SetBool("BattleEnd", true);
        /*if (Input.GetKeyDown(KeyCode.S))
        {
            gameObject.SetActive(true);
            _myAnim.Play("BattleTransition_EncounterInfo");
        }*/
            
    }

    void AssignBattleInfo()
    {
        Image[] sprites = _transitionBattleInfo.GetComponentsInChildren<Image>();
        //assign sprite
        sprites[1].sprite = boss.infoSprite[0];
        sprites[2].sprite = boss.infoSprite[1];
        
        //resizing
        foreach (Image img in sprites)
        {
            img.SetNativeSize();
        }
    }
    
    void BeginBattleEffectResize()
    {
        Image[] sprites = _transitionBattleEffect.GetComponentsInChildren<Image>();

        foreach (Image img in sprites)
        {
            img.SetNativeSize();
        }
        
        _magicParticle[0].SetActive(true);
    }

    void turnOnCanvas()
    {
        GameManager.Instance.StartBattle();
        
        mainCanvas = BattleManager.Instance._mainCanvas.GetComponent<CanvasGroup>();
        mainCanvas.DOFade(1, 0.8f);
        mainCanvas.interactable = true;
    }

    void turnOffCanvas()
    {
        //turn off main canvas
        mainCanvas.interactable = false;
        mainCanvas.DOFade(0, 0.5f);
    }
    
    void EndBattleEffectResize()
    {
        Image[] sprites = _transitionBattleEffect.GetComponentsInChildren<Image>();

        foreach (Image img in sprites)
        {
            img.SetNativeSize();
        }
        
        _magicParticle[1].SetActive(true);
    }

    void setActiveSelf()
    {
        Destroy(_transitionBattleEffect.gameObject);
        Destroy(_transitionBattleInfo.gameObject);
        Destroy(_myAnim);
    }
}
