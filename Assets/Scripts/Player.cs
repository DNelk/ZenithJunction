using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private SpriteRenderer _sr;
    
    [SerializeField] private int _maxHP;
    private int _currentHP;
    //HP UI
    private Slider _healthBar;
    private Text _hpText;
    
    private void Awake()
    {
        _currentHP = _maxHP;
        _sr = GetComponent<SpriteRenderer>();
        _healthBar = GameObject.Find("PlayerHealth").GetComponent<Slider>();
        _healthBar.maxValue = _maxHP;
        _hpText = _healthBar.GetComponentInChildren<Text>();
        UpdateHealth();
    }

    public void TakeDamage(int damage)
    {
        _currentHP -= damage;
        _sr.DOColor(Color.red, 0.2f).OnComplete(()=>_sr.DOColor(Color.white, 0.5f));
        UpdateHealth();
        if (_currentHP <= 0)
            Utils.DisplayGameOver("You Died!");
    }

    private void UpdateHealth()
    {
        _healthBar.value = _currentHP;
        _hpText.text = _currentHP + "/" + _maxHP;
    }
}
