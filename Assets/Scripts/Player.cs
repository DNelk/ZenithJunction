using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private SkinnedMeshRenderer _mr;
    
    [SerializeField] private int _maxHP = 10;
    private int _currentHP;
    //HP UI
    private GameObject _healthBar;
    private float hp_OriginLength;
    private TMP_Text _hpText;
    private Transform[] _positions;
    private int _currentPos; public int Position => _currentPos;
    private Enemy _enemy;

    //Stats
    public Dictionary<StatType, Stat> ActiveStats = new Dictionary<StatType, Stat>();
    public Dictionary<StatType, Stat> BaseStats = new Dictionary<StatType, Stat>();
    
    private void Awake()
    {
        _currentHP = _maxHP;
        _mr = GetComponentInChildren<SkinnedMeshRenderer>();
        _healthBar = GameObject.Find("PlayerHealth").transform.Find("Fill Area").gameObject;
        hp_OriginLength = _healthBar.GetComponent<RectTransform>().sizeDelta.x;
        _healthBar.transform.parent.GetComponent<HealthBar>().Target = "Player";
        //_healthBar.maxValue = _maxHP;
        _hpText = _healthBar.transform.parent.transform.Find("Numbers").GetComponent<TMP_Text>();
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
        //_mr.material.DOColor(Color.red, 0.2f).OnComplete(()=>_mr.material.DOColor(Color.white, 0.5f));
        UpdateHealth();
        if (_currentHP <= 0)
            Utils.DisplayGameOver("Defeat!");
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
        move.Join(transform.DORotate(_positions[_currentPos].rotation.eulerAngles, 0.5f));
        return move.Duration();
    }

    #region Stats
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
    #endregion
}
