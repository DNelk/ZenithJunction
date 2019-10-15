﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    private Engine _playerAttack; public Engine PlayerAttack => _playerAttack;
    private EnemyAttack _enemyAttack;
    public Enemy CurrentEnemy;
    public Player Player;
    public int CurrentAether;
    public BattleStates BattleState;
    private int _clashingDamage;
    //UI
    private Button _finishEnginesButton;
    private Text _playerText;
    private Text _enemyText;
    private Text _resultText;
    private Text _clashText;
    
    //Battle Variables
    //Engines
    public Engine[] Engines;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if(Instance != this)
            Destroy(gameObject);

        Init();
    }

    private void Init()
    {
        Player = GameObject.FindWithTag("Player").GetComponent<Player>();
        _finishEnginesButton = GameObject.Find("ConfirmButton").GetComponent<Button>();
        _finishEnginesButton.onClick.AddListener(ConfirmEngines);
        _playerText = GameObject.Find("PlayerText").GetComponent<Text>();
        _enemyText = GameObject.Find("EnemyText").GetComponent<Text>();
        _resultText = GameObject.Find("ResultText").GetComponent<Text>();
        _clashText = GameObject.Find("ClashText").GetComponent<Text>();
        BattleState = BattleStates.MakingEngines;
        _playerAttack = null;
        
        Engines = new Engine[3];
        Engines[0] = GameObject.Find("Engine1").GetComponent<Engine>();
        Engines[1] = GameObject.Find("Engine2").GetComponent<Engine>();
        Engines[2] = GameObject.Find("Engine3").GetComponent<Engine>();
    }
    
    private void Start()
    {
        LoadEnemyAttack();
    }


    private void Update()
    {
        switch (BattleState)
        {
            case BattleStates.MakingEngines:
                break;
            case BattleStates.BuyingCards:
                break;
            case BattleStates.ChoosingAction:
                ChoosingActionUpdate();
                break;
        }
    }

    private void ChoosingActionUpdate()
    {
        if (EmptyEnginesCount() == 3)
        {
            foreach (Engine e in Engines)
            {
                e.ToggleMode();
            }
            DeckManager.Instance.Reset();
            _clashText.text = "";
            _clashingDamage = 0;
            BattleState = BattleStates.MakingEngines;
            _finishEnginesButton.GetComponentInChildren<Text>().text = "Finish Engines";
            _finishEnginesButton.onClick.RemoveAllListeners();
            _finishEnginesButton.GetComponent<Image>().color = Color.white;
            _finishEnginesButton.onClick.AddListener(ConfirmEngines);
            
        }
    }

    private void LoadEnemyAttack()
    {
        if(CurrentEnemy == null)
            CurrentEnemy = GameObject.FindWithTag("Enemy").GetComponent<Enemy>();
        CurrentEnemy.PrepareAttack();
    }
    
    public void PushAttack(EnemyAttack a)
    {
        _enemyAttack = a;
    }

    public void PushAttack(Engine s)
    {
        if (_playerAttack != null && _playerAttack != s)
        {
            _playerAttack.Deselect();
        }
        _playerAttack = s;
    }

    public IEnumerator ProcessAttacks()
    {
        BattleState = BattleStates.Battle;
        
        _playerAttack.StartCoroutine(_playerAttack.ExecuteStack());
        yield return new WaitUntil(() =>_playerAttack.Executed);
        
        int playerDamage = _playerAttack.AtkTotal;
        CurrentAether = _playerAttack.AetherTotal;
        
        _playerText.text = "You attack for " + playerDamage + " damage. You have " + CurrentAether + " aether to spend.";
        Tween expandTextTween = _playerText.transform.parent.DOScale(1.5f, 0.5f);
        yield return expandTextTween.WaitForCompletion();
        _playerText.transform.parent.transform.DOScale(1f, 0.5f);
        
        int enemyDamage = CurrentEnemy.Attack();
        _enemyText.text = "The enemy attacks for " + enemyDamage + " damage.";
        expandTextTween = _enemyText.transform.parent.DOScale(1.5f, 0.5f);
        yield return expandTextTween.WaitForCompletion();
        _enemyText.transform.parent.transform.DOScale(1f, 0.5f);
        
        DisplayAttackResult(playerDamage, enemyDamage);
        expandTextTween = _resultText.transform.parent.DOScale(1.5f, 0.5f);
        yield return expandTextTween.WaitForCompletion();
        expandTextTween = _resultText.transform.parent.transform.DOScale(1f, 0.5f);
        yield return expandTextTween.WaitForCompletion();
        
        if (_playerAttack.MoveTotal > 0)
        {
            MovePlayerDialog mpd = Instantiate(Resources.Load<GameObject>("prefabs/moveplayerdialog"), GameObject.Find("Canvas").transform).GetComponent<MovePlayerDialog>();
            mpd.MoveTotal = _playerAttack.MoveTotal;
            yield return new WaitUntil(() => mpd.Confirmed);
            Destroy(mpd.gameObject);
        }
        
        yield return new WaitForSeconds(1.0f);
        //Load next enemy attack
        LoadEnemyAttack();
        _playerText.text = "";
        _resultText.text = "";
        _playerAttack = null;

        BattleState = BattleStates.BuyingCards;
            //Load Buy Manager
        BuyManager.Instance.StartCoroutine(BuyManager.Instance.LoadBuyMenu());
    }

    /*With "trampling
    private void DisplayAttackResult(int playerDamage, int enemyDamage)
    {
        int damage = Math.Abs(playerDamage - enemyDamage);
        if (playerDamage > enemyDamage)
        {
            _resultText.text = "The enemy takes " + damage + " damage!";
            _resultText.color = Color.green;
            _currentEnemy.TakeDamage(damage);
        }
        else if (playerDamage < enemyDamage)
        {
            _resultText.text = "You take " + damage + " damage!";
            _resultText.color = Color.red;
            Player.TakeDamage(damage);
        }
        else
        {
            _resultText.color = Color.yellow;
            _resultText.text = "It's a tie! The attacks bounce off!";
        }
    }
    */
    
    /*Without "trampling*/
    private void DisplayAttackResult(int playerDamage, int enemyDamage)
    {
        if (playerDamage > enemyDamage)
        {
            _resultText.text = "The enemy takes " + (playerDamage + _clashingDamage) + " damage!";
            _resultText.color = Color.green;
            CurrentEnemy.TakeDamage(playerDamage + _clashingDamage);
            _clashingDamage = 0;
            _clashText.text = "";

        }
        else if (playerDamage < enemyDamage)
        {
            _resultText.text = "You take " + (enemyDamage + _clashingDamage) + " damage!";
            _resultText.color = Color.red;
            Player.TakeDamage(enemyDamage + _clashingDamage);
            _clashingDamage = 0;
            _clashText.text = "";

        }
        else if(playerDamage != 0)
        {
            _resultText.color = Color.yellow;
            if (Utils.FlipCoin())
            {
                _resultText.text = "It's a tie! The attacks clash! Clash damage +" + playerDamage;
                _clashingDamage += playerDamage;
                _clashText.text = "Clash Power: " + _clashingDamage + "!";
            }
            else
            {
                _resultText.text = "It's a tie! The attacks bounce off!";
            }
        }
    }
    
    
    private void ConfirmEngines()
    {
        foreach (Engine e in Engines)
        {
            if (e.PendingCount > 3 || e.PendingCount < 3)
            {
                Utils.DisplayError("Engines must be exactly 3 cards!", 3f);
                return;
            }
        }

        BattleState = BattleStates.ChoosingAction;
        OrganizeEngines();
    }

    private void OrganizeEngines()
    {
        for(int i = 0; i < Engines.Length; i++)
        {
            Engine e = Engines[i];
            /*Vector3 targetPos = transform.position + (Vector3.down * i * 150);
            Tween moveStack = stack.transform.DOMove(targetPos, 0.5f, false);
            yield return moveStack.WaitForCompletion();
            */
            e.StackCards();
        }

        _finishEnginesButton.GetComponentInChildren<Text>().text = "Confirm Action";
        _finishEnginesButton.GetComponent<Image>().color = Color.yellow;
        _finishEnginesButton.onClick.RemoveAllListeners();
        _finishEnginesButton.onClick.AddListener(ConfirmAction);
    }

    private int EmptyEnginesCount()
    {
        int count = 0;
        foreach (Engine e in Engines)
        {
            if (e.Stack.Count == 0 && e.PendingCount == 0)
                count++;
        }

        return count;
    }

    public Engine GetNextOpenEngine()
    {
        for (int i = 0; i < Engines.Length; i++)
        {
            Engine currentEngine = Engines[i];
            if (currentEngine.PendingCount < 3)
            {
                return currentEngine;
            }
        }
        return null;
    }
    
    private void ConfirmAction()
    {
        if (_playerAttack == null)
        {
            Utils.DisplayError("No Action Selected", 3f);
            return;
        }
        
        StartCoroutine(ProcessAttacks());
    }
}

public enum BattleStates
{
    MakingEngines,
    ChoosingAction,
    BuyingCards,
    Battle
}

//Used for both players and enemies
public enum AttackRange
{
    Melee,
    Short,
    Long,
    Null,
    Inescapable
}