using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClashUIManager : MonoBehaviour
{
    //Singleton
    public static ClashUIManager Instance;
    
    //UI    
    private PowerBanner _playerBanner;
    private PowerBanner _enemyBanner;

    private Animator _anim;

    private TMP_Text _result;

    //For determining animation that will play
    private string _animTrigger = "";

    public bool AnimDone;
    
    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if(Instance != this)
            Destroy(gameObject);
        
        _playerBanner = transform.Find("PlayerPowerBanner").GetComponent<PowerBanner>();
        _enemyBanner = transform.Find("EnemyPowerBanner").GetComponent<PowerBanner>();

        _anim = GetComponent<Animator>();

        _result = transform.Find("Result").GetComponentInChildren<TMP_Text>();

    }

    public void TriggerClash(int playerDamage, int enemyDamage)
    {
        int dmg = 0;

        if (playerDamage == 0)
            _playerBanner.Text = "0";
        else
            _playerBanner.Text = playerDamage + " power";
        
        if (enemyDamage == 0)
            _enemyBanner.Text = "0";
        else
            _enemyBanner.Text = enemyDamage + " power";
        
        if (playerDamage > enemyDamage)
        {
            dmg = playerDamage - enemyDamage;
            _result.text = "You deal " + playerDamage + " damage!";
            
            if (playerDamage >= 2 * enemyDamage)
                _animTrigger = "PlayerWinBig";
            else
                _animTrigger = "PlayerWinSmall";
        }
        else if (enemyDamage > playerDamage)
        {
            dmg = playerDamage - enemyDamage;
            _result.text = "The enemy deals " + enemyDamage + " damage!";
            
            if (enemyDamage >= 2 * playerDamage)
                _animTrigger = "EnemyWinBig";
            else
                _animTrigger = "EnemyWinSmall";
        }
        else if (playerDamage != 0)
        {
            _animTrigger = "Draw";
            _result.text = "Draw!";
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
    }

    public void ClashDone()
    {
        _anim.SetTrigger("EndClash");
    }
}
