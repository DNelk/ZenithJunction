using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClashUIManager : MonoBehaviour
{
    //Singleton
    public static ClashUIManager Instance;
    
    //UI    
    private PowerBanner _playerBanner;
    private PowerBanner _enemyBanner;
    private PowerMeter _playerMeter;
    private PowerMeter _enemyMeter;
    private Image _crashBG;

    private Animator _anim;

    private TMP_Text _result;
    private TMP_FontAsset[] _damageNumberFont = new TMP_FontAsset[3];

    //For determining animation that will play
    private string _animTrigger = "";

    public bool AnimDone;
    
    //Other vars
    private ShakeCamera _cameraShaker;

    private void Update()
    {
        /*//just for debug
        if (Input.GetKeyDown(KeyCode.A))
        {
            _anim.SetTrigger("StartClash");
            _anim.SetTrigger("PlayerWinSmall");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            _anim.SetTrigger("StartClash");
            _anim.SetTrigger("PlayerWinBig");
        }*/
    } //for debug purpose only
    
    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if(Instance != this)
            Destroy(gameObject);
        
        _playerBanner = transform.Find("PlayerPowerBanner").GetComponent<PowerBanner>();
        _enemyBanner = transform.Find("EnemyPowerBanner").GetComponent<PowerBanner>();
        _playerMeter = transform.Find("PlayerPowerMeter").GetComponent<PowerMeter>();
        _enemyMeter = transform.Find("EnemyPowerMeter").GetComponent<PowerMeter>();

        _crashBG = transform.Find("BG").GetComponent<Image>();
        _crashBG.color = new Color(0,0,0,0);
        
        _anim = GetComponent<Animator>();

        _result = transform.Find("Result").GetComponentInChildren<TMP_Text>();
        _damageNumberFont[0] = Resources.Load<TMP_FontAsset>("Fonts/Palmmy/Soviet_Enumber");
        _damageNumberFont[1] = Resources.Load<TMP_FontAsset>("Fonts/Palmmy/Soviet_Dnumber");
        _damageNumberFont[2] = Resources.Load<TMP_FontAsset>("Fonts/Palmmy/Soviet_Draw");

        _cameraShaker = Camera.main.GetComponent<ShakeCamera>();
    }

    public void TriggerClash(int playerDamage, int enemyDamage)
    {
        int dmg = 0;
        _crashBG.DOColor(new Color(0, 0, 0, 0.8f), 0.5f);
        _crashBG.raycastTarget = true; //call this to prevent interaction with engine and card during attack

        if (playerDamage == 0)
            _playerBanner.Text = "0";
        else
            _playerBanner.Text = playerDamage + "";
        
        if (enemyDamage == 0)
            _enemyBanner.Text = "0";
        else
            _enemyBanner.Text = enemyDamage + "";
        
        if (playerDamage > enemyDamage)
        {
            dmg = playerDamage - enemyDamage;
            _result.text = "" + playerDamage + "";
            if (playerDamage >= 2 * enemyDamage)
                _animTrigger = "PlayerWinBig";
            else
                _animTrigger = "PlayerWinSmall";
        }
        else if (enemyDamage > playerDamage)
        {
            dmg = playerDamage - enemyDamage;
            _result.text = "" + enemyDamage + "";
            
            if (enemyDamage >= 2 * playerDamage)
                _animTrigger = "EnemyWinBig";
            else
                _animTrigger = "EnemyWinSmall";
        }
        else if (playerDamage != 0)
        {
            _animTrigger = "Draw";
            _result.text = "Draw";
        }
        
        _anim.SetTrigger("StartClash");
    }

    public void StartClashAnim()
    {
        _anim.SetTrigger(_animTrigger);
    }

    public void ContinueBattle()
    {
        AnimDone = true;
        _crashBG.raycastTarget = false;
    }

    public void ClashDone()
    {
        _anim.SetTrigger("EndClash");
        _crashBG.DOColor(new Color(0, 0, 0, 0), 0.5f);
    }

    public void PlaySpark(AnimationEvent evt)
    {
        int meterIndex = evt.intParameter;
        int sparkIndex = (int) evt.floatParameter;
        
        PowerMeter chosenMeter;

        if (meterIndex == 0)
            chosenMeter= _playerMeter;
        else if (meterIndex == 1)
            chosenMeter = _enemyMeter;
        else
            return;
        
        chosenMeter.PlaySpark(sparkIndex);
    }

    public void PlayAllSparks()
    {
        for (int i = 0; i < _playerMeter.Sparks.Length; i++)
        {
            _playerMeter.PlaySpark(i);
        }
        
        for (int i = 0; i < _enemyMeter.Sparks.Length; i++)
        {
            _enemyMeter.PlaySpark(i);
        }
    }
    
    public void PlayAllSparksInOne(AnimationEvent evt)
    {
        int meterIndex = evt.intParameter;
        
        PowerMeter chosenMeter;

        if (meterIndex == 0)
            chosenMeter= _playerMeter;
        else if (meterIndex == 1)
            chosenMeter = _enemyMeter;
        else
            return;
        
        for (int i = 0; i < chosenMeter.Sparks.Length; i++)
        {
            chosenMeter.PlaySpark(i);
        }
    }

    public void ShakeCamera()
    {
        _cameraShaker.CameraShake(.3f, 20f);
    }

    private void changeResultFont(int font)
    {
        _result.font = _damageNumberFont[font];
    }

    private void changeResultSize(int fontSize)
    {
        _result.fontSize = fontSize;
    }
}
