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

    private Engine _playerAttack; public Engine PlayerAttack => _playerAttack;
    private EnemyAttack _enemyAttack;
    public Enemy CurrentEnemy;
    public Player Player;
    public int CurrentAether;
    
    public BattleStates BattleState;
    private int _clashingDamage;
    //UI
    private Button _finishEnginesButton;
    private TMP_Text _confirmButtonText;
    //private UIPopIn _playerText;
    //private UIPopIn _enemyText;
    //private UIPopIn _resultText;
    //private UIPopIn _clashText;
    
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
        _confirmButtonText = _finishEnginesButton.GetComponentInChildren<TMP_Text>();
        _finishEnginesButton.onClick.AddListener(ConfirmEngines);
        _finishEnginesButton.gameObject.SetActive(false);
        
        //_playerText = GameObject.Find("PlayerText").GetComponent<UIPopIn>();
        //_enemyText = GameObject.Find("EnemyText").GetComponent<UIPopIn>();
        //_resultText = GameObject.Find("ResultText").GetComponent<UIPopIn>();
        //_clashText = GameObject.Find("ClashText").GetComponent<UIPopIn>();
        BattleState = BattleStates.MakingEngines;
        _playerAttack = null;
        
        Engines = new Engine[3];
        Engines[0] = GameObject.Find("Engine1").GetComponent<Engine>();
        Engines[1] = GameObject.Find("Engine2").GetComponent<Engine>();
        Engines[2] = GameObject.Find("Engine3").GetComponent<Engine>();
    }
    
    private void Start()
    {
        LoadAllEnemyAttacks();
    }


    private void Update()
    {
        switch (BattleState)
        {
            case BattleStates.MakingEngines:
                MakingEnginesUpdate();
                break;
            case BattleStates.BuyingCards:
                break;
            case BattleStates.ChoosingAction:
                ChoosingActionUpdate();
                break;
        }
    }

    private void MakingEnginesUpdate()
    {
        bool enginesDone = true;
        foreach (Engine e in Engines)
        {
            if (e.PendingCount > 3 || e.PendingCount < 3)
            {
                enginesDone = false;
            }
        }

        if (enginesDone)
        {
            _finishEnginesButton.gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ConfirmEngines();
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
            CurrentEnemy.PrintNext3();
            //_clashText.Alpha = 0;
            _clashingDamage = 0;
            BattleState = BattleStates.MakingEngines;
            _confirmButtonText.text = "Confirm Engines";
            _finishEnginesButton.onClick.RemoveAllListeners();
            _finishEnginesButton.onClick.AddListener(ConfirmEngines);
            _finishEnginesButton.gameObject.SetActive(false);
            
        }
        else
        {
            if(_enemyAttack == null)
                LoadEnemyAttack();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ConfirmAction();
        }
    }

    private void LoadEnemyAttack()
    {
        if(CurrentEnemy == null)
            CurrentEnemy = GameObject.FindWithTag("Enemy").GetComponent<Enemy>();
        CurrentEnemy.PrepareAttack();
    }
    
    private void LoadAllEnemyAttacks()
    {
        if(CurrentEnemy == null)
            CurrentEnemy = GameObject.FindWithTag("Enemy").GetComponent<Enemy>();
        CurrentEnemy.PrintNext3();
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
        CurrentEnemy.HideIntentions();
        Utils.DestroyCardPreview();
        _confirmButtonText.text = "";
        
        /***Player go***/
        _playerAttack.StartCoroutine(_playerAttack.ExecuteStack());
        yield return new WaitUntil(() =>_playerAttack.Executed);
        //Apply any debuffs
        BattleDelegateHandler.ApplyNegativeEnemyEffects();
        
        //Allow player movement
        if (_playerAttack.MoveTotal > 0)
        {
            MovePlayerDialog mpd = Instantiate(Resources.Load<GameObject>("prefabs/moveplayerdialog"), GameObject.Find("MainCanvas").transform).GetComponent<MovePlayerDialog>();
            mpd.MoveTotal = _playerAttack.MoveTotal;
            yield return new WaitUntil(() => mpd.Confirmed);
            Destroy(mpd.gameObject);
        }

        //Finish Executing
        _playerAttack.CalculatePowerTotal();
        
        int playerDamage = _playerAttack.PowerTotal;
        if (playerDamage < 0)
            playerDamage = 0;
        CurrentAether = _playerAttack.AetherTotal;
        
        
        /***Enemy go***/
        
        int enemyDamage = CurrentEnemy.Attack();
        if (enemyDamage < 0)
            enemyDamage = 0;
        //Move Enemy
        BattleDelegateHandler.MoveEnemy();

        //Resolve attacks
        if (playerDamage != 0 || enemyDamage != 0)
        {
            ClashUIManager.Instance.TriggerClash(playerDamage, enemyDamage);
            yield return new WaitUntil(() => ClashUIManager.Instance.AnimDone);
            ClashUIManager.Instance.AnimDone = false;
        }

        DisplayAttackResult(playerDamage, enemyDamage);
        
        //yield return new WaitForSeconds(2.0f);
        //Load next enemy attack
        Player.TickDownStats();
        _enemyAttack = null;
        CurrentEnemy.TickDownStats();

        _playerAttack = null;
        
        _confirmButtonText.text = "Select an Engine";
        
        //Load Buy Manager
        if ((CurrentAether > 0 ||
            BuyManager.Instance.FreeBuysRemaining > 0) && BuyManager.Instance.BuysRemaining != 0)
        {
            BuyManager.Instance.LoadShop();
        }
        else
        {
            BattleState = BattleStates.ChoosingAction;
            BuyManager.Instance.RotateRow();
            BuyManager.Instance.BuysRemaining = -1;
            BuyManager.Instance.FreeBuysRemaining = 0;

        }
    
    }

    /*Without "trampling*/
    private void DisplayAttackResult(int playerDamage, int enemyDamage)
    {
        if (playerDamage > enemyDamage)
        {
           //_resultText.Text = "The enemy takes " + (playerDamage + _clashingDamage) + " damage!";
           //_resultText.Color = Color.green;
            CurrentEnemy.TakeDamage(playerDamage + _clashingDamage);
            _clashingDamage = 0;
           // _clashText.Alpha = 0;

        }
        else if (playerDamage < enemyDamage)
        {
            //_resultText.Text = "You take " + (enemyDamage +  _clashingDamage) + " damage!";
            //_resultText.Color = Color.red;
            Player.TakeDamage(enemyDamage + _clashingDamage);
            _clashingDamage = 0;
            //_clashText.Alpha = 0;
        }
        else
        {
            //_resultText.Color = Color.yellow;
            if(playerDamage != 0 && false)
            {
                //_resultText.PopIn();
                //_resultText.Text = "It's a tie! The attacks clash! Clash damage +" + playerDamage;
                _clashingDamage += playerDamage;
                //_clashText.Alpha = 1;
                //_clashText.Text = "Clash Power: " + _clashingDamage + "!";
            }
            else
            {
                //_resultText.Text = "It's a tie! The attacks bounce off!";
            }
        }
        //_resultText.PopIn();
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
        LoadEnemyAttack();
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

        _confirmButtonText.text = "Select an Engine";
        _finishEnginesButton.onClick.RemoveAllListeners();
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

    public void EngineSelected()
    {
        _confirmButtonText.text = "Confirm Engine";
        _finishEnginesButton.onClick.AddListener(ConfirmAction);
    }
    
    private void ConfirmAction()
    {
        if(BattleState != BattleStates.ChoosingAction)
            return;
        
        if (_playerAttack == null)
        {
            Utils.DisplayError("No Action Selected", 3f);
            return;
        }
        _finishEnginesButton.onClick.RemoveAllListeners();
        StartCoroutine(ProcessAttacks());
    }
}

public enum BattleStates
{
    MakingEngines,
    ChoosingAction,
    BuyingCards,
    Battle,
    GameOver
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