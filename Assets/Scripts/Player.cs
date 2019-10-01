using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int _maxHP;
    private SpriteRenderer _sr;
    private int _currentHP;

    private void Awake()
    {
        _currentHP = _maxHP;
        _sr = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        _currentHP -= damage;
        _sr.DOColor(Color.red, 0.2f).OnComplete(()=>_sr.DOColor(Color.white, 0.5f));
        if (_currentHP <= 0)
            Utils.DisplayGameOver("You Died!");
    }
}
