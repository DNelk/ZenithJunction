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
    
    //for check Mouse Drag
    public bool isMouseDragging = false;

    private Engine _playerAttack; public Engine PlayerAttack => _playerAttack;
    private EnemyAttack _enemyAttack;
    public Enemy CurrentEnemy;
    public Player Player;
    public int CurrentAether;

    public BattleStates BattleState = BattleStates.BattleStart;
    private int _clashingDamage;
    //UI
    public Button ConfirmButton;
    private ConfirmButtonEventManager _confirmEvent;
    private Image _confirmButtonText;
    private Sprite[] _confirmButtonSprite = new Sprite[2];
    public int NumEngines = 3;

    private Image _confirmCore;

    private Animator _commenceAnim;

    public HealthBar playerHealthBar; //for player health bar ref
    public HealthBar enemyHealthBar; //for enemy health bar ref
    //private UIPopIn _playerText;
    //private UIPopIn _enemyText;
    //private UIPopIn _resultText;
    //private UIPopIn _clashText;
    
    //Battle Variables
    //Engines
    public Engine[] Engines;
    //set up canvas
    public Transform _mainCanvas;
    //set move 
    private MovePlayerDialog _moveUI;
    private Animator _moveUIAura;
    //set EnemyPos Ui
    public EnemyPosition enemyPos;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if(Instance != this)
            Destroy(gameObject);
        
        
        //get enemy
        if (CurrentEnemy == null)
            CurrentEnemy = GameObject.FindWithTag("Enemy").GetComponent<Enemy>();
    }

    public void Init()
    {
        Player = GameObject.FindWithTag("Player").GetComponent<Player>();
        
        
        ConfirmButton = GameObject.Find("ConfirmButton").GetComponent<Button>();
        _confirmEvent = ConfirmButton.transform.GetComponent<ConfirmButtonEventManager>();
        _confirmButtonText = ConfirmButton.GetComponentsInChildren<Image>()[1];
        //_confirmCore = ConfirmButton.transform.parent.transform.Find("Core").GetComponent<Image>();
        ConfirmButton.onClick.AddListener(ConfirmEngines);
        ConfirmButton.gameObject.SetActive(false);
        _commenceAnim = ConfirmButton.transform.parent.GetComponent<Animator>();
        _confirmButtonSprite[0] = Resources.Load<Sprite>("Sprites/Core/Gear");
        _confirmButtonSprite[1] = Resources.Load<Sprite>("Sprites/Core/Gear2");

        playerHealthBar = GameObject.Find("PlayerHealth").GetComponent<HealthBar>();
        enemyHealthBar = GameObject.Find("EnemyHealth").GetComponent<HealthBar>();

        //_playerText = GameObject.Find("PlayerText").GetComponent<UIPopIn>();
        //_enemyText = GameObject.Find("EnemyText").GetComponent<UIPopIn>();
        //_resultText = GameObject.Find("ResultText").GetComponent<UIPopIn>();
        //_clashText = GameObject.Find("ClashText").GetComponent<UIPopIn>();
        //BattleState = BattleStates.MakingEngines;
        
        _playerAttack = null;
        
        Engines = new Engine[3];
        Engines[0] = GameObject.Find("Engine1").GetComponent<Engine>();
        Engines[1] = GameObject.Find("Engine2").GetComponent<Engine>();
        Engines[2] = GameObject.Find("Engine3").GetComponent<Engine>();
        
        //GameManager.Instance.StartBattle();
        
        //get move UI
        _mainCanvas = GameObject.Find("MainCanvas").transform;
        Transform posPanel = _mainCanvas.Find("PositionsPanel");
        //get move set
        Transform moveSet = posPanel.transform.Find("MoveButton");
        _moveUI = moveSet.GetComponent<MovePlayerDialog>();
        _moveUIAura = _moveUI.transform.parent.transform.Find("PlayerPos").GetComponentInChildren<Animator>();
        //get enemyPos
        enemyPos = posPanel.transform.Find("EnemyPos").GetComponent<EnemyPosition>();
    }
    
    private void Start()
    {
        Init();
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
            default:
                break;
        }
        
        
    }

    private void MakingEnginesUpdate()
    {
        bool enginesDone = true;
        foreach (Engine e in Engines)
        {
            if (e.PendingCount > 3 || e.PendingCount < 3 && DeckManager.Instance.CardsToBeSorted.Count != 0)
            {
                enginesDone = false;
                _commenceAnim.SetBool("TurnOn", false);
                _confirmButtonText.sprite = _confirmButtonSprite[0];
                ConfirmButton.gameObject.SetActive(false);
            }
        }

        if (enginesDone)
        {
            ConfirmButton.gameObject.SetActive(true);
            _commenceAnim.SetBool("TurnOn", true);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ConfirmEngines();
        }
    }
    private void ChoosingActionUpdate()
    {
        if (EmptyEnginesCount() == NumEngines)
        {
            foreach (Engine e in Engines)
            {
                e.ToggleMode();
            }
            if (TutorialManager.Instance != null && TutorialManager.Instance.TriggerAfterBattle)
                TutorialManager.Instance.Step();
            else
                DeckManager.Instance.Reset();
            CurrentEnemy.PrintNext3();
            //_clashText.Alpha = 0;
            _clashingDamage = 0;
            //_confirmEvent.turnOff();
            //_confirmEvent.reset();
            BattleState = BattleStates.MakingEngines;
            StartCoroutine(ReadEngines());
            //_confirmButtonText.text = "Confirm Engines";
            _commenceAnim.SetBool("TurnOn", false);
            ConfirmButton.onClick.RemoveAllListeners();
            ConfirmButton.onClick.AddListener(ConfirmEngines);
            ConfirmButton.gameObject.SetActive(false);
            DamageDealtThisTurn = 0;
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

    public int DamageDealtThisTurn = 0;
    public IEnumerator ProcessAttacks()
    {
        BattleState = BattleStates.Battle;
        CurrentEnemy.HideIntentions();
        Utils.DestroyCardPreview();
        //_confirmButtonText.text = "";
        
        /***Player go***/
        _playerAttack.StartCoroutine(_playerAttack.ExecuteStack());
        yield return new WaitUntil(() =>_playerAttack.Executed);
        //Apply any debuffs
        BattleDelegateHandler.ApplyNegativeEnemyEffects();
        
        //Allow player movement
        if (_playerAttack.MoveTotal > 0)
        {
            _moveUI.MoveTotal = _playerAttack.MoveTotal;
            _moveUI.gameObject.SetActive(true);
            _moveUIAura.SetBool("isAnimate", true);

            BattleState = BattleStates.Moving;
            
            yield return new WaitUntil(() => _moveUI.Confirmed);

            _moveUI.Confirmed = false;
            
            BattleState = BattleStates.Battle;

            //new one
            _moveUIAura.SetBool("isAnimate", false);
            _moveUI.gameObject.SetActive(false);
        }

        //Finish Executing
        _playerAttack.CalculatePowerTotal();
        
        int playerDamage = _playerAttack.PowerTotal;
        if (playerDamage < 0)
            playerDamage = 0;
        CurrentAether = _playerAttack.AetherTotal;
        _playerAttack.UpdateUICounts(true);
        
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
        else
        {
            playerHealthBar.UpdateStatusChanges();
            enemyHealthBar.UpdateStatusChanges();
        }

        DisplayAttackResult(playerDamage, enemyDamage);
        
        //yield return new WaitForSeconds(2.0f);
        //Load next enemy attack
        Player.TickDownStats();
        _enemyAttack = null;
        CurrentEnemy.TickDownStats();
        
        BattleDelegateHandler.ApplyAfterDamageEffects();
        
        //_confirmButtonText.text = "Select an Engine";
        
        //Load Buy Manager
        if ((CurrentAether > 0 ||
            BuyManager.Instance.FreeBuysRemaining > 0) && BuyManager.Instance.BuysRemaining != 0)
        {
            BuyManager.Instance.LoadShop();
        }
        else
        {
            BattleState = BattleStates.ChoosingAction;
            _confirmButtonText.sprite = _confirmButtonSprite[1];
            _confirmEvent.turnOn();
            BuyManager.Instance.RotateRow();
            BuyManager.Instance.BuysRemaining = -1;
            BuyManager.Instance.FreeBuysRemaining = 0;
        }
        
        _playerAttack.StateChange(0);
        _playerAttack.isActive = false;
        _playerAttack = null;
    }

    /*Without "trampling*/
    private void DisplayAttackResult(int playerDamage, int enemyDamage)
    {
        if (playerDamage > enemyDamage)
        {
           //_resultText.Text = "The enemy takes " + (playerDamage + _clashingDamage) + " damage!";
           //_resultText.Color = Color.green;
            CurrentEnemy.TakeDamage(playerDamage + _clashingDamage);
            DamageDealtThisTurn += playerDamage + _clashingDamage;
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
            if (e.PendingCount > 3 || e.PendingCount < 3 && DeckManager.Instance.CardsToBeSorted.Count != 0)
            {
                Utils.DisplayError("Engines must be exactly 3 cards!", 3f);
                return;
            }
        }

        BattleState = BattleStates.ChoosingAction;
        DeckManager.Instance.lockTab();
        DeckManager.Instance.playLockTabParticle();
        _confirmButtonText.sprite = _confirmButtonSprite[1];
        _confirmEvent.setTrigger();
        _commenceAnim.SetTrigger("CoreTrigger");
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

        //_confirmButtonText.text = "Select an Engine";
        ConfirmButton.onClick.RemoveAllListeners();
    }

    public int EmptyEnginesCount()
    {
        int count = 0;
        foreach (Engine e in Engines){
            if (e.Stack.Count == 0 && e.PendingCount == 0 && !e.EmptyStack)
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
        _confirmButtonText.sprite = _confirmButtonSprite[0]; 
        _confirmEvent.setTrigger();
        ConfirmButton.onClick.AddListener(ConfirmAction);
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
        ConfirmButton.onClick.RemoveAllListeners();
        _confirmEvent.turnOff();
        _commenceAnim.SetTrigger("CoreTrigger");
        StartCoroutine(ProcessAttacks());
    }
    
    public void SetConfirmOn()
    {
        _confirmButtonText.sprite = _confirmButtonSprite[1];
        _confirmEvent.turnOn();
    }

    public void InitializeBattle()
    {
        BattleState = BattleStates.MakingEngines;
        Player.TurnONHPGlow();
        CurrentEnemy.TurnONHPGlow();
        _moveUIAura.SetTrigger("activateMoveUI");

        StartCoroutine(ReadEngines());
    }

    private IEnumerator ReadEngines()
    {
        for (int i = 0; i < Engines.Length; i++)
        {
            Engines[i].isActive = true;
            Engines[i].StateChange(1);
            yield return new WaitForSeconds(0.1f);
        }
    }
}

public enum BattleStates
{
    MakingEngines,
    ChoosingAction,
    BuyingCards,
    Battle,
    GameOver,
    BattleStart,
    Moving,
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