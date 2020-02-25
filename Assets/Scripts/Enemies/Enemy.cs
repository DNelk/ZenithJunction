using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected string _name; 
    [SerializeField] protected List<EnemyAttack> _attacks; //Our pool of attacks
    [SerializeField] protected EnemyAttack _currentAttack;
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
    public Dictionary<StatType, Stat> ActiveStats = new Dictionary<StatType, Stat>();
    public Dictionary<StatType, Stat> BaseStats = new Dictionary<StatType, Stat>();
    
    //HP UI
    private GameObject _healthBar;
    private float hp_OriginLength;
    private TMP_Text _hpText;
    
    private void Awake()
    {
        _atkIndex = -1;
        _currentHP = _maxHP;
       // _mr = GetComponent<MeshRenderer>();
        _healthBar = GameObject.Find("EnemyHealth").transform.Find("Fill Area").gameObject;
        hp_OriginLength = _healthBar.GetComponent<RectTransform>().sizeDelta.x;
//        Debug.Log(hp_OriginLength);
        //_healthBar.maxValue = _maxHP;
        _healthBar.transform.parent.GetComponent<HealthBar>().Target = "Enemy";
        _hpText = _healthBar.transform.parent.transform.Find("Numbers").GetComponent<TMP_Text>();
        UpdateHealth();
        _positions = new []{GameObject.Find("EnemyPos1").transform, GameObject.Find("EnemyPos2").transform, GameObject.Find("EnemyPos3").transform};
        _currentPos = 0;
        if(_moveInRange == null)
            _moveInRange = Resources.Load<GameObject>("Prefabs/Enemies/BasicAttacks/MoveInAttackRange").GetComponent<EnemyAttack>();
        _enemyIntention = GameObject.Find("EnemyIntentions").GetComponent<EnemyIntentionUI>();
    }

    public void PrepareAttack()
    {
        _enemyIntention.gameObject.SetActive(true);
        int tempAtkIndex = _atkIndex + 1;
        if (tempAtkIndex >= _attacks.Count)
            tempAtkIndex = 0;
        _currentAttack = _attacks[tempAtkIndex];
        //If this attack is out of range we should just move towards the player instead
        if (_currentAttack.IsAttackInRange())
            _atkIndex = tempAtkIndex;
        else
        {
            DesiredRange = _currentAttack.Range;
            _currentAttack = _moveInRange; 
        }
        //Display how much damage/what type of attack it's going to deal
        //Text enemyText = GameObject.Find("EnemyText").GetComponent<Text>();
        _enemyIntention.Text = "The enemy prepares to " + _currentAttack.PrepareAttack();
        BattleManager.Instance.PushAttack(_currentAttack);
        
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
        _currentAttack.ExecuteOtherEffect();
        int dmg = CalculateAttackTotalWithPosition(_currentAttack);
        dmg -= AtkDebuff;
        AtkDebuff = 0;
        return dmg;
    }

    public void TakeDamage(int damage)
    {

        //Defense Stat check!
        if (ActiveStats.ContainsKey(StatType.DefenseUP))
        {
            if(!ActiveStats[StatType.DefenseUP].IsNew)
                damage -= ActiveStats[StatType.DefenseUP].Value;
        }
        if (ActiveStats.ContainsKey(StatType.DefenseDOWN))
        {
            if(!ActiveStats[StatType.DefenseDOWN].IsNew)
                damage += ActiveStats[StatType.DefenseDOWN].Value;
        }

        damage = CalculateDamageWithStatus(damage);
        
        if (damage < 0)
            damage = 0;
        _currentHP -= damage;
        //_mr.material.DOColor(Color.red, 0.2f).OnComplete(() => _mr.material.DOColor(Color.white, 0.5f));
        UpdateHealth();
        if (_currentHP <= 0)
            Utils.DisplayGameOver("Victory!");
    }
    
    private void UpdateHealth()
    {
        //_healthBar.value = _currentHP;
        _healthBar.GetComponent<RectTransform>().sizeDelta = new Vector2( (((_currentHP*100)/_maxHP) * hp_OriginLength)/100, _healthBar.GetComponent<RectTransform>().sizeDelta.y);
        _hpText.text = _currentHP + "/" + _maxHP;
    }
    
    public float ChangePosition(int newPos)
    {
        if(newPos >= _positions.Length)
            return -1;
        _currentPos = newPos;
        Sequence move = DOTween.Sequence();
        move.Append(transform.DOMove(_positions[_currentPos].position, 0.5f));
        move.Insert(0,transform.DORotate(_positions[_currentPos].rotation.eulerAngles, 0.5f));
        return move.Duration();
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
                    damageMod = 0.5f;
                break;
            default:
                damageMod = 1;
                break;
        }

        return (int)(currentAttack.TotalDamage * damageMod);
    }

    public IEnumerator MoveInRange(int moves = 3)
    {
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
    }

    protected virtual int CalculateDamageWithStatus(int dmg)
    {
        return dmg;
    }

    public void ModifyStat(StatType type, int turnsLeft, int value, bool applyImmidiately = false)
    {
        if (ActiveStats.ContainsKey(type))
        {
            if (ActiveStats[type].Value == 0)
                ActiveStats[type].IsNew = true;
            ActiveStats[type].Value += value;
            ActiveStats[type].TurnsLeft += turnsLeft;
        }
        else
        {
            ActiveStats.Add(type, new Stat(turnsLeft, value, applyImmidiately));
        }
            
    }

    public void TickDownStats()
    {
        foreach (var stat in ActiveStats.Values)
        {
            if (stat.IsNew)
            {
                stat.IsNew = false;
                continue;
            }
            
            if (stat.TurnsLeft > 0)
            {
                stat.TurnsLeft--;
            }
            
            if(stat.TurnsLeft == 0)
            {
                stat.Value = 0;
            }
        }
    }
}