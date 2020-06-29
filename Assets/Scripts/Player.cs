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
    private Transform _takingDamageAnim;
    private RectTransform[] _damageUnit = new RectTransform[3];
    private float[] _damageUnitWidth = new float[3];
    private List<GameObject> _hpGlowEffectUnit = new List<GameObject>();
    private TMP_Text _realDamageText;
    
    //Stats
   // public Dictionary<StatType, Stat> ActiveStats = new Dictionary<StatType, Stat>();
    public List<Stat> ActiveStatsList = new List<Stat>();
    public Dictionary<StatType, Stat> BaseStats = new Dictionary<StatType, Stat>();

    private Quaternion _prevRot;
    private bool _isRotating;

    private void Update()
    {
        //UpdateHealth();
        
        Vector3 dir = _enemy.transform.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(dir);
        if (_prevRot == null)
            _prevRot = rot;
        if (rot != _prevRot && !_isRotating)
        {
            _isRotating = true;
            transform.DORotate(rot.eulerAngles, 0.5f).OnComplete(() => _isRotating = false);
        }
        
        //for debug pure pose only
        if (Input.GetKeyDown(KeyCode.A))
        {
            TakeDamage(6);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            TakeDamage(5);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            TakeDamage(4);
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            TakeDamage(19);
        }
    }
    
    private void Awake()
    {
        _currentHP = _maxHP;
        _mr = GetComponentInChildren<SkinnedMeshRenderer>();

        Transform hpBar = GameObject.Find("PlayerHealth").transform;
        _healthBarFill = hpBar.Find("Fill Area").gameObject;
        _hpBar = _healthBarFill.transform.Find("HP").gameObject;
        _hpBarRect = _hpBar.GetComponent<RectTransform>();
        _hpBarPos = _hpBarRect.localPosition;
        _hpBarWidth = _hpBarRect.rect.width;
        
        hp_OriginLength = _healthBarFill.GetComponent<RectTransform>().sizeDelta.x;
        _healthBar = _healthBarFill.transform.parent.GetComponent<HealthBar>();
        _healthBar.Target = "Player";
        //_healthBar.maxValue = _maxHP;
        _hpText = _healthBarFill.transform.parent.transform.Find("Numbers").GetComponent<TMP_Text>();
        UpdateHealth(0, _maxHP);
        _positions = new []{GameObject.Find("PlayerPos1").transform, GameObject.Find("PlayerPos2").transform, GameObject.Find("PlayerPos3").transform};
        _currentPos = 0;
        _enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>();
        
        //for taking damage anim
        _takingDamageAnim = hpBar.Find("LosingHPAnimation").GetComponent<RectTransform>();
        Transform hpGlow = hpBar.Find("HealthGlowAnimation").transform;
        for (int i = 0; i < 20; i++)
        {
            _hpGlowEffectUnit.Add(hpGlow.GetChild(i).gameObject);
        }
        _damageUnit[0] = _takingDamageAnim.transform.Find("Unit").GetComponent<RectTransform>();
        _damageUnit[1] = _takingDamageAnim.transform.Find("Crack").GetComponent<RectTransform>();
        _damageUnit[2] = _takingDamageAnim.transform.Find("Crack2").GetComponent<RectTransform>();
        for (int i = 0; i < _damageUnit.Length ; i++)
        {
            _damageUnitWidth[i] = _damageUnit[i].rect.width;
        }
        _realDamageText = _damageUnit[0].GetComponentInChildren<TMP_Text>(); // assign damage number shown in animation 

    }

    public void TakeDamage(int damage)
    {
        //Defense Stat check!
        Stat s;
        damage = StatManager.Instance.StatCheck(damage, ActiveStatsList, StatType.DefenseDOWN, StatType.DefenseUP);
      
        if (damage <= 0) return; //it will never heal you

        int oldHp = _currentHP;
        _currentHP -= damage;
        //_mr.material.DOColor(Color.red, 0.2f).OnComplete(()=>_mr.material.DOColor(Color.white, 0.5f));
        UpdateHealth(damage, oldHp);
        if (_currentHP <= 0)
            Utils.DisplayGameOver("Defeat!", false);
    }

    public void GainLife(int heal)
    {
        _currentHP += heal;
        if (_currentHP > _maxHP)
            _currentHP = _maxHP;
    }
    
    private void UpdateHealth(int damage, int oldHp)
    {
        //change position of the fill gauge
        float oldXpos = _hpBarRect.position.x; //set up the old xPos for HP before decreased
        float x_pos = 0;

        //see if HP 0 or not
        if (_currentHP <= 0) x_pos = _hpBarPos.x - (_hpBarWidth * 0.975f);
        else x_pos = _hpBarPos.x - (((float)(_maxHP - _currentHP)/_maxHP) * (_hpBarWidth * 0.975f));
        
        _hpBarRect.localPosition = new Vector3(x_pos,_hpBarPos.y, _hpBarPos.z); //move the bar
        if (_currentHP <= 0) _hpText.text = 0 + "/" + _maxHP; //update new HP text
        else _hpText.text = _currentHP + "/" + _maxHP;
        float newXpos = _hpBarRect.position.x; //remember the new xPos for HP after decreased

        //taking attack animation
        if (damage > 0)
        {
            float trueDamage = 0;
            if (_currentHP <= 0) trueDamage = (float)oldHp;
            else trueDamage = damage;
            
            //change takingDamageAnim position
            Vector3 takingDamagePos = _takingDamageAnim.position; //take ref for position
            _takingDamageAnim.position = new Vector3(((oldXpos + newXpos)/2) - 5, takingDamagePos.y, takingDamagePos.z ); //set new position
            
            float scaleOffset = 0;
            float newWidth = 0;
            
            //for original unit
            scaleOffset = _damageUnitWidth[0] * (((((float) trueDamage / _maxHP) * 20) - 1) * 0.5f);
            if (scaleOffset >= 0) newWidth = _damageUnitWidth[0] + scaleOffset;
            else newWidth = _damageUnitWidth[0];
            _damageUnit[0].sizeDelta = new Vector2(newWidth, _damageUnit[0].rect.height);
            
            //for left cracked unit , offset scale multiplier is (((damage / maxHp) * 20) - 1) * 0.255f;
            scaleOffset = _damageUnitWidth[1] * ((((((float) trueDamage / _maxHP) * 20) - 3) * 0.255f) + 0.3f);
            if (scaleOffset >= 0) newWidth = _damageUnitWidth[1] + scaleOffset;
            else newWidth = _damageUnitWidth[1];
            _damageUnit[1].sizeDelta = new Vector2(newWidth, _damageUnit[1].rect.height);
            
            //for right cracked unit , offset scale multiplier is (((damage / maxHp) * 20) - 1) * 0.265f;
            scaleOffset = _damageUnitWidth[2] * ((((((float) trueDamage / _maxHP) * 20) - 3) * 0.265f) + 0.27f);
            if (scaleOffset >= 0) newWidth = _damageUnitWidth[2] + scaleOffset;
            else newWidth = _damageUnitWidth[2];
            _damageUnit[2].sizeDelta = new Vector2(newWidth, _damageUnit[2].rect.height);

            _realDamageText.text = "-" + trueDamage; //assign damage number
            _takingDamageAnim.gameObject.SetActive(true); //then play the animation
            
            //set the glow animation to decrease number
            for (int i = _currentHP; i < _currentHP + trueDamage; i++)
            {
                _hpGlowEffectUnit[i].SetActive(false);
            }
        }
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
        
        StatManager.Instance.ModifyStat(ActiveStatsList, type, turnsLeft, value, _healthBar, applyImmidiately);
        
        /*if (ActiveStats.ContainsKey(type))
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
        
        _healthBar.UpdateStatusChanges();*/
    }
    
    public void TickDownStats()
    {
        
        StatManager.Instance.TickDownStats(ActiveStatsList, _healthBar);
        /*foreach (var stat in ActiveStats.Values)
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
        
        _healthBar.UpdateStatusChanges();*/
    }
    #endregion
}
