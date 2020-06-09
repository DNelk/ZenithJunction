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
    private HealthBar _healthBar;
    private GameObject _healthBarFill;
    private GameObject _hpBar;
    private RectTransform _hpBarRect;
    private Vector3 _hpBarPos;
    private float _hpBarWidth;
    private float hp_OriginLength;
    private TMP_Text _hpText;
    private Transform[] _positions;
    private int _currentPos; public int Position => _currentPos;
    private Enemy _enemy;

    //Stats
    public Dictionary<StatType, Stat> ActiveStats = new Dictionary<StatType, Stat>();
    public Dictionary<StatType, Stat> BaseStats = new Dictionary<StatType, Stat>();

    private Quaternion _prevRot;
    private bool _isRotating;
    private void Update()
    {
        UpdateHealth();
        
        Vector3 dir = _enemy.transform.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(dir);
        if (_prevRot == null)
            _prevRot = rot;
        if (rot != _prevRot && !_isRotating)
        {
            _isRotating = true;
            transform.DORotate(rot.eulerAngles, 0.5f).OnComplete(() => _isRotating = false);
        }
    }
    
    private void Awake()
    {
        _currentHP = _maxHP;
        _mr = GetComponentInChildren<SkinnedMeshRenderer>();
            
        _healthBarFill = GameObject.Find("PlayerHealth").transform.Find("Fill Area").gameObject;
        _hpBar = _healthBarFill.transform.Find("HP").gameObject;
        _hpBarRect = _hpBar.GetComponent<RectTransform>();
        _hpBarPos = _hpBarRect.localPosition;
        _hpBarWidth = _hpBarRect.rect.width;
        
        hp_OriginLength = _healthBarFill.GetComponent<RectTransform>().sizeDelta.x;
        _healthBar = _healthBarFill.transform.parent.GetComponent<HealthBar>();
        _healthBar.Target = "Player";
        //_healthBar.maxValue = _maxHP;
        _hpText = _healthBarFill.transform.parent.transform.Find("Numbers").GetComponent<TMP_Text>();
        UpdateHealth();
        _positions = new []{GameObject.Find("PlayerPos1").transform, GameObject.Find("PlayerPos2").transform, GameObject.Find("PlayerPos3").transform};
        _currentPos = 0;
        _enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>();
    }

    public void TakeDamage(int damage)
    {
        //Defense Stat check!
        Stat s;
        if (ActiveStats.TryGetValue(StatType.DefenseUP, out s))
            if(!s.IsNew)damage -= s.Value;
        if (ActiveStats.TryGetValue(StatType.DefenseDOWN, out s))
            if(!s.IsNew)damage += s.Value;
      
        _currentHP -= damage;
        //_mr.material.DOColor(Color.red, 0.2f).OnComplete(()=>_mr.material.DOColor(Color.white, 0.5f));
        UpdateHealth();
        if (_currentHP <= 0)
            Utils.DisplayGameOver("Defeat!", false);
    }

    public void GainLife(int heal)
    {
        _currentHP += heal;
        if (_currentHP > _maxHP)
            _currentHP = _maxHP;
    }
    
    private void UpdateHealth()
    {
        //_healthBar.value = _currentHP;
        float x_pos = _hpBarPos.x - (((float)(_maxHP - _currentHP)/_maxHP)* _hpBarWidth);
        _hpBarRect.localPosition = new Vector3(x_pos,_hpBarPos.y, _hpBarPos.z);
        _hpText.text = _currentHP + "/" + _maxHP;
    }

    public float ChangePosition(int newPos)
    {
        if(newPos >= _positions.Length)
            return -1;
        _currentPos = newPos;
        transform.parent = transform.parent.parent.parent.Find("Car" + (_currentPos + 1)).transform.Find("train").transform;
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
            ActiveStats.Add(type, new Stat(turnsLeft, value, applyImmidiately, type));
        }
        
        _healthBar.UpdateStatusChanges();
            
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
        
        _healthBar.UpdateStatusChanges();
    }
    #endregion
}
