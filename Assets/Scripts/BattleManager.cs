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
    public Player Player;
    public int CurrentSteam;
    public BattleStates BattleState;

    //UI
    private Button _finishEnginesButton;
    private Text _playerText;
    private Text _enemyText;
    private Text _resultText;
    
    //Battle Variables
    
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
        BattleState = BattleStates.MakingEngines;
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
        if (DeckManager.Instance.EmptyEnginesCount() == 3)
        {
            DeckManager.Instance.Reset();
            BattleState = BattleStates.MakingEngines;
            _finishEnginesButton.GetComponentInChildren<Text>().text = "Finish Engines";
            _finishEnginesButton.onClick.RemoveAllListeners();
            _finishEnginesButton.GetComponent<Image>().color = Color.white;
            _finishEnginesButton.onClick.AddListener(ConfirmEngines);
            
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
        Tween expandTextTween = _playerText.transform.DOScale(2f, 0.5f);
        yield return expandTextTween.WaitForCompletion();
        _playerText.transform.DOScale(1f, 0.5f);
        
        int enemyDamage = _currentEnemy.Attack();
        _enemyText.text = "The enemy attacks for " + enemyDamage + " damage.";
        expandTextTween = _enemyText.transform.DOScale(2f, 0.5f);
        yield return expandTextTween.WaitForCompletion();
        _enemyText.transform.DOScale(1f, 0.5f);
        
        DisplayAttackResult(playerDamage, enemyDamage);
        expandTextTween = _resultText.transform.DOScale(2f, 0.5f);
        yield return expandTextTween.WaitForCompletion();
        _resultText.transform.DOScale(1f, 0.5f);
        
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

    private void ConfirmEngines()
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
        OrganizeEngines();
    }

    private void OrganizeEngines()
    {
        for(int i = 0; i < DeckManager.Instance.Engines.Length; i++)
        {
            EngineManager stack = DeckManager.Instance.Engines[i];
            /*Vector3 targetPos = transform.position + (Vector3.down * i * 150);
            Tween moveStack = stack.transform.DOMove(targetPos, 0.5f, false);
            yield return moveStack.WaitForCompletion();
            */
            stack.StackCards();
        }

        _finishEnginesButton.GetComponentInChildren<Text>().text = "Confirm Action";
        _finishEnginesButton.GetComponent<Image>().color = Color.yellow;
        _finishEnginesButton.onClick.RemoveAllListeners();
        _finishEnginesButton.onClick.AddListener(ConfirmAction);
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