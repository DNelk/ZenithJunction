using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public string Target = "";
    public Tooltip _tooltip;

    private GameObject _attackUp, _attackDown, _moveUp, _moveDown, _defUp, _defDown;
    private Animator _atkAnim, _moveAnim, _defAnim;
    private int _atkValue, _moveValue, _defValue;

    private void Awake()
    {
        //set all the stat changes object
        Transform myStatChanges = transform.Find("StatChanges");
        
        _attackUp = myStatChanges.transform.Find("Attack_Up").gameObject;
        _attackDown = myStatChanges.transform.Find("Attack_Down").gameObject;
        _moveUp = myStatChanges.transform.Find("Move_Up").gameObject;
        _moveDown = myStatChanges.transform.Find("Move_Down").gameObject;
        _defUp = myStatChanges.transform.Find("Def_Up").gameObject;
        _defDown = myStatChanges.transform.Find("Def_Down").gameObject;

        //Set anim
        Transform myAnimStat = transform.Find("StatChangesAnimation");

        _atkAnim = myAnimStat.transform.Find("AttackChange").GetComponent<Animator>();
        _moveAnim = myAnimStat.transform.Find("MoveChange").GetComponent<Animator>();
        _defAnim = myAnimStat.transform.Find("DefChange").GetComponent<Animator>();
    }

    private void Update()
    {
        //for debug purpose
        /*if (Target == "Player")
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                BattleManager.Instance.Player.ModifyStat(StatType.AttackUP, 0, 1, false);
                BattleManager.Instance.Player.ModifyStat(StatType.DefenseUP, 0, 1, false);
                BattleManager.Instance.Player.ModifyStat(StatType.MovesUP, 0, 1, false);
                UpdateStatusChanges();
            }
            
            if (Input.GetKeyDown(KeyCode.S))
            {
                BattleManager.Instance.Player.ModifyStat(StatType.AttackDOWN, 0, 1, false);
                BattleManager.Instance.Player.ModifyStat(StatType.DefenseDOWN, 0, 1, false);
                BattleManager.Instance.Player.ModifyStat(StatType.MovesDOWN, 0, 1, false);
                UpdateStatusChanges();
            }
        }*/
        
        /*if (Target == "Enemy")
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                BattleManager.Instance.CurrentEnemy.ModifyStat(StatType.AttackUP, 0, 1, false);
                BattleManager.Instance.CurrentEnemy.ModifyStat(StatType.DefenseUP, 0, 1, false);
                BattleManager.Instance.CurrentEnemy.ModifyStat(StatType.MovesUP, 0, 1, false);
                UpdateStatusChanges();
            }
            
            if (Input.GetKeyDown(KeyCode.S))
            {
                BattleManager.Instance.CurrentEnemy.ModifyStat(StatType.AttackDOWN, 0, 1, false);
                BattleManager.Instance.CurrentEnemy.ModifyStat(StatType.DefenseDOWN, 0, 1, false);
                BattleManager.Instance.CurrentEnemy.ModifyStat(StatType.MovesDOWN, 0, 1, false);
                UpdateStatusChanges();
            }
        }*/
    } //for debug purpose only

    public void UpdateStatusChanges()
    {
        if(Target == "") //if there is no player or enemy to get stats from, ditch it
            return;

        List<Stat> stats = new List<Stat>(); //make this variable to store the stats changes
        
        if (Target == "Player") //if it's a player, pull from player model
            stats = BattleManager.Instance.Player.ActiveStatsList;
        else if (Target == "Enemy") //vice versa for enemy
            stats = BattleManager.Instance.CurrentEnemy.ActiveStatsList;
        else //if none then ditch it again
            return;
        
        int value = 0; //use this to store the value of stat changes on each type
        Stat currentStat; //use this to 
        TMP_Text statNumber = null;
        
        //Attack
        value = StatManager.Instance.StatCheck(value, stats, StatType.AttackUP, StatType.AttackDOWN);
        
        //attack changes
        if (value < _atkValue)
        {
            _atkAnim.gameObject.SetActive(true);
            _atkAnim.Play("StatDown_Play");
        }
        else if (value > _atkValue)
        {
            _atkAnim.gameObject.SetActive(true);
            _atkAnim.Play("StatUp_Play");
        }
        
        if (value != 0)
        {
            if (value < 0)
            {
                statNumber = _attackDown.transform.GetComponentInChildren<TMP_Text>();
                if (!_attackDown.activeSelf)
                {   
                    _attackDown.SetActive(true);
                    statNumber.text = value.ToString();
                }
                else statNumber.text = value.ToString();

                if (_attackUp.activeSelf) _attackUp.SetActive(false);
            }
            else if (value > 0)
            {
                statNumber = _attackUp.transform.GetComponentInChildren<TMP_Text>();
                if (!_attackUp.activeSelf)
                {   
                    _attackUp.SetActive(true);
                    statNumber.text = value.ToString();
                }
                else statNumber.text = value.ToString();
                
                if (_attackDown.activeSelf) _attackDown.SetActive(false);
            }
        }
        else
        {
            if (_attackUp.activeSelf) _attackUp.SetActive(false);
            if (_attackDown.activeSelf) _attackDown.SetActive(false);
        }

        _atkValue = value;
        value = 0;
        
        //def
        value = StatManager.Instance.StatCheck(value, stats, StatType.DefenseUP, StatType.DefenseDOWN);
        
        //def changes
        if (value < _defValue)
        {
            _defAnim.gameObject.SetActive(true);
            _defAnim.Play("StatDown_Play");
        }
        else if (value > _moveValue)
        {
            _defAnim.gameObject.SetActive(true);
            _defAnim.Play("StatUp_Play");
        }
        
        if (value != 0)
        {
            if (value < 0)
            {
                statNumber = _defDown.transform.GetComponentInChildren<TMP_Text>();
                if (!_defDown.activeSelf)
                {   
                    _defDown.SetActive(true);
                    statNumber.text = value.ToString();
                }
                else statNumber.text = value.ToString();

                if (_defUp.activeSelf) _defUp.SetActive(false);
            }
            else if (value > 0)
            {
                statNumber = _defUp.transform.GetComponentInChildren<TMP_Text>();
                if (!_defUp.activeSelf)
                {   
                    _defUp.SetActive(true);
                    statNumber.text = value.ToString();
                }
                else statNumber.text = value.ToString();
                
                if (_defDown.activeSelf) _defDown.SetActive(false);
            }
        }
        else
        {
            if (_defUp.activeSelf) _defUp.SetActive(false);
            if (_defDown.activeSelf) _defDown.SetActive(false);
        }
        
        _defValue = value;
        value = 0;
        
        //move
        value = StatManager.Instance.StatCheck(value, stats, StatType.MovesUP, StatType.MovesDOWN);
        
        //move changes
        if (value < _moveValue)
        {
            _moveAnim.gameObject.SetActive(true);
            _moveAnim.Play("StatDown_Play");
        }
        else if (value > _moveValue)
        {
            _moveAnim.gameObject.SetActive(true);
            _moveAnim.Play("StatUp_Play");
        }
        
        if (value != 0)
        {
            if (value < 0)
            {
                statNumber = _moveDown.transform.GetComponentInChildren<TMP_Text>();
                if (!_moveDown.activeSelf)
                {   
                    _moveDown.SetActive(true);
                    statNumber.text = value.ToString();
                }
                else statNumber.text = value.ToString();

                if (_moveDown.activeSelf) _moveUp.SetActive(false);
            }
            else if (value > 0)
            {
                statNumber = _moveUp.transform.GetComponentInChildren<TMP_Text>();
                if (!_moveUp.activeSelf)
                {   
                    _moveUp.SetActive(true);
                    statNumber.text = value.ToString();
                }
                else statNumber.text = value.ToString();
                
                if (_moveDown.activeSelf) _moveDown.SetActive(false);
            }
        }
        else
        {
            if (_moveUp.activeSelf) _moveUp.SetActive(false);
            if (_moveDown.activeSelf) _moveDown.SetActive(false);
        }

        _moveValue = value;
        value = 0;
        
        //tell engine to update its attack power
        /*if (Target == "Player")
        {
            foreach (Engine e in BattleManager.Instance.Engines)
            {
                if (e.EngineState == EngineState.Stacked)
                {
                    //Debug.Log(e);
                    //e.UpdateUICounts();
                }
            }
        }*/
    }
    
    public void ShowPreview()
    {
        if(Target == "")
            return;
        
        string displayStr = "";

        List<Stat> stats = new List<Stat>();
        
        if (Target == "Player")
            stats = BattleManager.Instance.Player.ActiveStatsList;
        else if (Target == "Enemy")
            stats = BattleManager.Instance.CurrentEnemy.ActiveStatsList;
        else
            return;
        
        int value = 0;
        Stat currentStat;
        
        //Attack
        value = StatManager.Instance.StatCheck(value, stats, StatType.AttackUP, StatType.AttackDOWN);

        if (value != 0)
        {
            displayStr += "\nAttack ";
            if (value < 0)
                displayStr += value;
            else if (value > 0)
                displayStr += "+" + value;
            value = 0;
        }

        //Defense
        value = StatManager.Instance.StatCheck(value, stats, StatType.DefenseUP, StatType.DefenseDOWN);

        if (value != 0)
        {
            displayStr += "\nDefense ";
            if (value < 0)
                displayStr += value;
            else if (value > 0)
                displayStr += "+" + value;
            value = 0;
        }
        
        //Move
        value = StatManager.Instance.StatCheck(value, stats, StatType.MovesUP, StatType.MovesDOWN);

        if (value != 0)
        {
            displayStr += "\nMove ";
            if (value < 0)
                displayStr += value;
            else if (value > 0)
                displayStr += "+" + value;
            value = 0;
        }
        
        if (Target == "Enemy")
            displayStr += "\n" + BattleManager.Instance.CurrentEnemy.ExtraInfo;
        
        if(displayStr == "")
            return;

        _tooltip = Instantiate(Resources.Load<GameObject>("prefabs/UI/HealthBar/tooltip"), transform.GetChild(0).transform.position,
            Quaternion.identity, transform).GetComponent<Tooltip>();
        _tooltip.Text = "Status:" + displayStr;
    }
    
    public void HidePreview()
    {
        if(_tooltip == null)
            return;
        _tooltip.StartCoroutine(_tooltip.FadeOut());
        _tooltip = null;
    }

    public void ActiveHPBar()
    {
        
    }
}
