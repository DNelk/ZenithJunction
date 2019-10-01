using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected string _name; 
    [SerializeField] protected List<Attack> _attacks; //Our pool of attacks
    protected int _atkIndex; //Which attack r we doing
    [SerializeField] protected int _maxHP;
    protected int _currentHP;
    private MeshRenderer _mr;
    private void Awake()
    {
        _atkIndex = -1;
        _currentHP = _maxHP;
        _mr = GetComponent<MeshRenderer>();
    }

    public void PrepareAttack()
    {
        _atkIndex++;
        if (_atkIndex >= _attacks.Count)
            _atkIndex = 0;
        Attack currentAttack = _attacks[_atkIndex];
        //Display how much damage/what type of attack it's going to deal
        Text enemyText = GameObject.Find("EnemyText").GetComponent<Text>();
        enemyText.text = "The enemy prepares to " + currentAttack.PrepareAttack();
        BattleManager.Instance.PushAttack(currentAttack);
    }
    
    public int Attack()
    {
        Attack currentAttack = _attacks[_atkIndex];
        //Deal damage and execute any other effects.
        currentAttack.ExecuteOtherEffect();
        return currentAttack.TotalDamage;
    }

    public void TakeDamage(int damage)
    {
        _currentHP -= damage;
        _mr.material.DOColor(Color.red, 0.2f).OnComplete(() => _mr.material.DOColor(Color.white, 0.5f));
        if (_currentHP <= 0)
            Utils.DisplayGameOver("Enemy Defeated!");
    }
}
