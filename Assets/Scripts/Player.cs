using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private MeshRenderer _mr;
    
    [SerializeField] private int _maxHP = 10;
    private int _currentHP;
    //HP UI
    private Slider _healthBar;
    private TMP_Text _hpText;
    private Transform[] _positions;
    private int _currentPos; public int Position => _currentPos;
    private Enemy _enemy;
    
    private void Awake()
    {
        _currentHP = _maxHP;
        _mr = GetComponent<MeshRenderer>();
        _healthBar = GameObject.Find("PlayerHealth").GetComponent<Slider>();
        _healthBar.maxValue = _maxHP;
        _hpText = _healthBar.transform.Find("Numbers").GetComponent<TMP_Text>();
        UpdateHealth();
        _positions = new []{GameObject.Find("PlayerPos1").transform, GameObject.Find("PlayerPos2").transform, GameObject.Find("PlayerPos3").transform};
        _currentPos = 0;
        _enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>();

    }

    /*private void Update()
    {
        Vector3 dir = _enemy.transform.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = rot;
    }*/

    public void TakeDamage(int damage)
    {
        _currentHP -= damage;
        _mr.material.DOColor(Color.red, 0.2f).OnComplete(()=>_mr.material.DOColor(Color.white, 0.5f));
        UpdateHealth();
        if (_currentHP <= 0)
            Utils.DisplayGameOver("You Died!");
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
        move.Join(transform.DORotate(_positions[_currentPos].rotation.eulerAngles, 0.5f));
        return move.Duration();
    }
}
