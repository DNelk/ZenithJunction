using System;
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

    private EngineManager _playerAttack; public EngineManager PlayerAttack => _playerAttack;
    private Attack _enemyAttack;
    private Enemy _currentEnemy;
    private Player _player;
    public int CurrentSteam;
    public BattleStates BattleState;

    //UI
    private Button _finishPilesButton;
    private Text _playerText;
    private Text _enemyText;
    private Text _resultText;

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
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        _finishPilesButton = GameObject.Find("ConfirmButton").GetComponent<Button>();
        _finishPilesButton.onClick.AddListener(ConfirmPiles);
        _playerText = GameObject.Find("PlayerText").GetComponent<Text>();
        _enemyText = GameObject.Find("EnemyText").GetComponent<Text>();
        _resultText = GameObject.Find("ResultText").GetComponent<Text>();
        BattleState = BattleStates.MakingPiles;
        _playerAttack = null;
    }
    
    private void Start()
    {
        LoadEnemyAttack();
    }


    private void Update()
    {
        switch (BattleState)
        {
            case BattleStates.MakingPiles:
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
        if (DeckManager.Instance.EmptyEnginesCount() == 3)
        {
            DeckManager.Instance.Reset();
            BattleState = BattleStates.MakingPiles;
        }
    }

    private void LoadEnemyAttack()
    {
        if(_currentEnemy == null)
            _currentEnemy = GameObject.FindWithTag("Enemy").GetComponent<Enemy>();
        _currentEnemy.PrepareAttack();
    }
    
    public void PushAttack(Attack a)
    {
        _enemyAttack = a;
    }

    public void PushAttack(EngineManager s)
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
        CurrentSteam = _playerAttack.SteamTotal;
        _playerText.text = "You attack for " + playerDamage + " damage. You have " + CurrentSteam + " steam to spend.";
        int enemyDamage = _currentEnemy.Attack();
        _enemyText.text = "The enemy attacks for " + enemyDamage + " damage.";
        
        DisplayAttackResult(playerDamage, enemyDamage);

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

    private void DisplayAttackResult(int playerDamage, int enemyDamage)
    {
        int damage = Math.Abs(playerDamage - enemyDamage);
        if (playerDamage > enemyDamage)
        {
            _resultText.text = "The enemy takes " + damage + " damage!";
            _currentEnemy.TakeDamage(damage);
        }
        else if (playerDamage < enemyDamage)
        {
            _resultText.text = "You take " + damage + " damage!";
            _player.TakeDamage(damage);
        }
        else
        {
            _resultText.text = "It's a tie! The attacks bounce off!";
        }
    }

    private void ConfirmPiles()
    {
        foreach (EngineManager stack in DeckManager.Instance.Engines)
        {
            if (stack.Count > 3 || stack.Count < 3)
            {
                Utils.DisplayError("Engines must be exactly 3 cards!", 3f);
                return;
            }
        }

        BattleState = BattleStates.ChoosingAction;
        StartCoroutine(OrganizePiles());
    }

    private IEnumerator OrganizePiles()
    {
        for(int i = 0; i < DeckManager.Instance.Engines.Length; i++)
        {
            EngineManager stack = DeckManager.Instance.Engines[i];
            Vector3 targetPos = transform.position + (Vector3.down * i * 150);
            Tween moveStack = stack.transform.DOMove(targetPos, 0.5f, false);
            yield return moveStack.WaitForCompletion();
            stack.StackCards();
        }

        _finishPilesButton.GetComponentInChildren<Text>().text = "Confirm Action";
        _finishPilesButton.onClick.RemoveAllListeners();
        _finishPilesButton.onClick.AddListener(ConfirmAction);
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
    MakingPiles,
    ChoosingAction,
    BuyingCards,
    Battle
}