using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int _maxHP;
    private int _currentHP;

    private void Awake()
    {
        _currentHP = _maxHP;
    }

    public void TakeDamage(int damage)
    {
        _currentHP -= damage;
    }
}
