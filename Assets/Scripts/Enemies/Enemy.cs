using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected string _name; 
    [SerializeField] protected List<EnemyAttack> _attacks; //Our pool of attacks
    [SerializeField] protected EnemyAttack _currentAttack;
    protected EnemyAttack _moveInRange; //Our move action that we can do when we need to get in range
    protected int _atkIndex; //Which attack r we doing
    [SerializeField] protected int _maxHP;
    protected int _currentHP;
    private MeshRenderer _mr;
    private Transform[] _positions;
    private int _currentPos; public int Position => _currentPos;
    [HideInInspector] public AttackRange DesiredRange = AttackRange.Null;
    public int AtkDebuff = 0;
    
    //HP UI
    private Slider _healthBar;
    private Text _hpText;
    
    private void Awake()
    {
        _atkIndex = -1;
        _currentHP = _maxHP;
        _mr = GetComponent<MeshRenderer>();
        _healthBar = GameObject.Find("EnemyHealth").GetComponent<Slider>();
        _healthBar.maxValue = _maxHP;
        _hpText = _healthBar.GetComponentInChildren<Text>();
        UpdateHealth();
        _positions = new []{GameObject.Find("EnemyPos1").transform, GameObject.Find("EnemyPos2").transform, GameObject.Find("EnemyPos3").transform};
        _currentPos = 0;
        _moveInRange = Resources.Load<GameObject>("Prefabs/Enemies/Attacks/MoveInAttackRange").GetComponent<EnemyAttack>();
    }

    public void PrepareAttack()
    {
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
        Text enemyText = GameObject.Find("EnemyText").GetComponent<Text>();
        enemyText.text = "The enemy prepares to " + _currentAttack.PrepareAttack();
        BattleManager.Instance.PushAttack(_currentAttack);
        
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
        _currentHP -= damage;
        _mr.material.DOColor(Color.red, 0.2f).OnComplete(() => _mr.material.DOColor(Color.white, 0.5f));
        UpdateHealth();
        if (_currentHP <= 0)
            Utils.DisplayGameOver("Enemy Defeated!");
    }
    
    private void UpdateHealth()
    {
        _healthBar.value = _currentHP;
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
            //Short range attacks are weaker if not right next
            case AttackRange.Short:
                if (distance == 1)
                    damageMod = 0.5f;
                else if (distance >= 2)
                    damageMod = 0;
                break;
            //Long range attacks are only good far away
            case AttackRange.Long:
                if (distance == 0)
                    damageMod = 0;
                break;
            default:
                damageMod = 1;
                break;
        }

        return (int)(currentAttack.TotalDamage * damageMod);
    }

    public IEnumerator MoveInRange()
    {
        Player player = BattleManager.Instance.Player;
        //We want to move closer
        if (DesiredRange == AttackRange.Melee || DesiredRange == AttackRange.Short)
        {
            //Player is further back
            if (player.Position > _currentPos)
            {
                while (player.Position > _currentPos)
                {
                    yield return new WaitForSeconds(ChangePosition(_currentPos + 1));
                }
            }
            //Player is further up front
            else
            {
                while (player.Position < _currentPos)
                {
                    yield return new WaitForSeconds(ChangePosition(_currentPos - 1));
                }
            }
        }
        //We want to move further
        else if (DesiredRange == AttackRange.Long)
        {
            if(player.Position == 0)
                yield return new WaitForSeconds(ChangePosition(_currentPos + 1));
            else
                yield return new WaitForSeconds(ChangePosition(_currentPos - 1));
        }
    }
    
}
