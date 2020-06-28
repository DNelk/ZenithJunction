using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected string _name; 
    [SerializeField] protected List<EnemyAttack> _attacks; //Our pool of attacks
    [SerializeField] protected string _description;
    public EnemyAttack CurrentAttack;
    [SerializeField] protected EnemyAttack _moveInRange; //Our move action that we can do when we need to get in range
    [SerializeField] public string ExtraInfo;
    
    protected int _atkIndex; //Which attack r we doing
    [SerializeField] protected int _maxHP;
    protected int _currentHP;
    //private MeshRenderer _mr;
    private Transform[] _positions;
    private int _currentPos; public int Position => _currentPos;
    [HideInInspector] public AttackRange DesiredRange = AttackRange.Null;
    public int AtkDebuff = 0;
    private EnemyIntentionUI _enemyIntention;
    
    //Stats
    //public Dictionary<StatType, Stat> ActiveStats = new Dictionary<StatType, Stat>();
    public List<Stat> ActiveStatsList = new List<Stat>();
    public Dictionary<StatType, Stat> BaseStats = new Dictionary<StatType, Stat>();
    
    //HP UI
    private HealthBar _healthBar;
    private GameObject _healthBarFill;
    private GameObject _hpBar;
    private RectTransform _hpBarRect;
    private Vector3 _hpBarPos;
    private float _hpBarWidth;
    private float hp_OriginLength;
    private TMP_Text _hpText;

    private void Awake()
    {
        _atkIndex = -1;
        _currentHP = _maxHP;
       // _mr = GetComponent<MeshRenderer>();
        _healthBarFill = GameObject.Find("EnemyHealth").transform.Find("Fill Area").gameObject;
        _hpBar = _healthBarFill.transform.Find("HP").gameObject;
        _hpBarRect = _hpBar.GetComponent<RectTransform>();
        _hpBarPos = _hpBarRect.localPosition;
        _hpBarWidth = _hpBarRect.rect.width;
        
        hp_OriginLength = _healthBarFill.GetComponent<RectTransform>().sizeDelta.x;
//        Debug.Log(hp_OriginLength);
        //_healthBar.maxValue = _maxHP;
        _healthBar = _healthBarFill.transform.parent.GetComponent<HealthBar>();
        _healthBar.Target = "Enemy";
        _hpText = _healthBarFill.transform.parent.transform.Find("Numbers").GetComponent<TMP_Text>();
        UpdateHealth();
        _positions = new []{GameObject.Find("EnemyPos1").transform, GameObject.Find("EnemyPos2").transform, GameObject.Find("EnemyPos3").transform};
        _currentPos = 0;
        if(_moveInRange == null)
            _moveInRange = Resources.Load<GameObject>("Prefabs/Enemies/BasicAttacks/MoveInAttackRange").GetComponent<EnemyAttack>();
        _enemyIntention = GameObject.Find("EnemyIntentions").GetComponent<EnemyIntentionUI>();
    }

    private void Start()
    {
        transform.parent = GameObject.Find("SceneRoot").transform.Find("Train").transform.Find("Car1");
    }

    private Quaternion _prevRot;
    private bool _isRotating;
    private void Update()
    {
        if (BattleManager.Instance.Player != null)
        {
            Vector3 dir = BattleManager.Instance.Player.transform.position - transform.position;
            Quaternion rot = Quaternion.LookRotation(dir);
            if (_prevRot == null)
                _prevRot = rot;
            if (rot != _prevRot && !_isRotating)
            {
                _isRotating = true;
                transform.DORotate(rot.eulerAngles, 0.5f).OnComplete(() => _isRotating = false);
            }
        }
    }

    public void PrepareAttack()
    {
        _enemyIntention.gameObject.SetActive(true);
        int tempAtkIndex = _atkIndex + 1;
        if (tempAtkIndex >= _attacks.Count)
            tempAtkIndex = 0;
        CurrentAttack = _attacks[tempAtkIndex];
        //If this attack is out of range we should just move towards the player instead
        if (CurrentAttack.IsAttackInRange())
            _atkIndex = tempAtkIndex;
        else
        {
            DesiredRange = CurrentAttack.Range;
            CurrentAttack = _moveInRange; 
        }
        //Display how much damage/what type of attack it's going to deal
        //Text enemyText = GameObject.Find("EnemyText").GetComponent<Text>();
        _enemyIntention.Text = "The enemy prepares to " + CurrentAttack.PrepareAttack();
        BattleManager.Instance.PushAttack(CurrentAttack);
        
    }

    public void PrintNext3()
    {
        _enemyIntention.gameObject.SetActive(true);
        _enemyIntention.Text = "";
        EnemyAttack a;
        int atkI = _atkIndex + 1;
        for (int i = 0; i < 1; i++)
        {
            if (atkI >= _attacks.Count || atkI < 0)
                atkI = 0;
            a = _attacks[atkI];
            _enemyIntention.Text += a.Warning + "\n";
            atkI++;
        }
    }

    public void HideIntentions()
    {
        _enemyIntention.gameObject.SetActive(false);
    }
    
    public int Attack()
    {
        
        //Deal damage and execute any other effects.
        CurrentAttack.ExecuteOtherEffect();
        int dmg = CalculateAttackTotalWithPosition(CurrentAttack);
        if (dmg > 0)
        {
            dmg = StatManager.Instance.StatCheck(dmg, ActiveStatsList, StatType.AttackUP, StatType.AttackDOWN);
        }
        dmg -= AtkDebuff;
        AtkDebuff = 0;
        return dmg;
    }

    public void TakeDamage(int damage)
    {

        //Defense Stat check!
        damage = StatManager.Instance.StatCheck(damage, ActiveStatsList, StatType.DefenseDOWN, StatType.DefenseUP);
      
        damage = CalculateDamageWithStatus(damage);
        
        if (damage < 0)
            damage = 0;
        _currentHP -= damage;
        //_mr.material.DOColor(Color.red, 0.2f).OnComplete(() => _mr.material.DOColor(Color.white, 0.5f));
        UpdateHealth();
        if (_currentHP <= 0)
            Utils.DisplayGameOver("Victory!", true);
    }
    
    private void UpdateHealth()
    {
        //_healthBar.value = _currentHP;
        float x_pos = _hpBarPos.x + (((float)(_maxHP - _currentHP)/_maxHP)* _hpBarWidth);
        _hpBarRect.localPosition = new Vector3(x_pos, _hpBarPos.y, _hpBarPos.z);
        _hpText.text = _currentHP + "/" + _maxHP;
    }
    
    public float ChangePosition(int newPos)
    {
        if(newPos >= _positions.Length)
            return -1;
        _currentPos = newPos;
        transform.parent = transform.parent.parent.Find("Car" + (_currentPos + 1));
        Sequence move = DOTween.Sequence();
        move.Append(transform.DOMove(_positions[_currentPos].position, 0.5f));
        move.Insert(0,transform.DORotate(_positions[_currentPos].rotation.eulerAngles, 0.5f));
        return move.Duration();
    }

    public void PushPosition(int moveAmount)
    {
        bool backMovement = BattleManager.Instance.Player.Position > _currentPos;

        int pos = _currentPos;
        
        while (moveAmount > 0 && _currentPos != 0 && _currentPos != _positions.Length-1)
        {
            if (backMovement)
            {
                pos--;
                moveAmount--;
            }
            else
            {
                pos++;
                moveAmount--;
            }
        }

        ChangePosition(pos);
        BattleManager.Instance.enemyPos.UpdatePosition();
    }
    
    public int CalculateAttackTotalWithPosition(EnemyAttack currentAttack)
    {
        int distance = Mathf.Abs(BattleManager.Instance.Player.Position - BattleManager.Instance.CurrentEnemy.Position);
        float damageMod = 1;
        switch (currentAttack.Range)
        {
            //Melee Range Attacks only work right next to target
            case AttackRange.Melee:
                if (distance > 0)
                    damageMod = 0;
                break;
            //Short ranged attacks are weaker if not right next
            case AttackRange.Short:
                if (distance >= 2) 
                    damageMod = 0.5f;
                break;
            //Long range attacks are only good far away
            case AttackRange.Long:
                if (distance == 0)
                    damageMod = 0f;
                break;
            default:
                damageMod = 1;
                break;
        }

        return (int)(currentAttack.TotalDamage * damageMod);
    }
    
    public IEnumerator MoveInRange(int moves = 3)
    {
        //Stat Check
        moves = StatManager.Instance.StatCheck(moves, ActiveStatsList, StatType.MovesUP, StatType.MovesDOWN);

        Player player = BattleManager.Instance.Player;
        //We want to move closer
        if (DesiredRange == AttackRange.Melee || DesiredRange == AttackRange.Short)
        {
            //Player is further back
            if (player.Position > _currentPos)
            {
                while (player.Position > _currentPos && moves > 0)
                {
                    yield return new WaitForSeconds(ChangePosition(_currentPos + 1));
                    moves--;
                }
            }
            //Player is further up front
            else
            {
                while (player.Position < _currentPos && moves > 0)
                {
                    yield return new WaitForSeconds(ChangePosition(_currentPos - 1));
                    moves--;
                }
            }
        }
        //We want to move further
        else if (DesiredRange == AttackRange.Long && moves > 0)
        {
            if(player.Position == 0)
                yield return new WaitForSeconds(ChangePosition(_currentPos + 1));
            else
                yield return new WaitForSeconds(ChangePosition(_currentPos - 1));
            moves--;
        }
        
        BattleManager.Instance.enemyPos.UpdatePosition();
    }

    protected virtual int CalculateDamageWithStatus(int dmg)
    {
        return dmg;
    }

    public void ModifyStat(StatType type, int turnsLeft, int value, bool applyImmidiately = false)
    {
        StatManager.Instance.ModifyStat(ActiveStatsList, type, turnsLeft, value, _healthBar, applyImmidiately);
    }

    public void TickDownStats()
    {
        StatManager.Instance.TickDownStats(ActiveStatsList, _healthBar);
    }

    
    //for UI information of enemy
    #region UI Information
    public string getName()
    {
        return _name;
    }

    public string getDescription()
    {
        return _description;
    }

    public List<EnemyAttack> getAttack()
    {
        List<EnemyAttack> _allMoves = new List<EnemyAttack>(_attacks);
        _allMoves.Add(_moveInRange);
        
        return _allMoves;
    }

    public string getSpecial()
    {
        return ExtraInfo;
    }
    #endregion
}