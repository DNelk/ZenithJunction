using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public string Target = "";
    public Tooltip _tooltip;

    private GameObject attackUp, attackDown, moveUp, moveDown, defUp, defDown;

    private Transform _myStatChanges;

    private void Awake()
    {
        _myStatChanges = transform.Find("StatChanges");
        attackUp = _myStatChanges.transform.Find("Attack_Up").gameObject;
        attackDown = _myStatChanges.transform.Find("Attack_Down").gameObject;
        moveUp = _myStatChanges.transform.Find("Move_Up").gameObject;
        moveDown = _myStatChanges.transform.Find("Move_Down").gameObject;
        defUp = _myStatChanges.transform.Find("Def_Up").gameObject;
        defDown = _myStatChanges.transform.Find("Def_Down").gameObject;
    }

    private void Update()
    {
        //for debug purpose
        if (Target == "Player")
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
        }
        
        if (Target == "Enemy")
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
        }
    }

    public void UpdateStatusChanges()
    {
        if(Target == "") //if there is no player or enemy to get stats from, ditch it
            return;

        Dictionary<StatType, Stat> stats; //make this variable to store the stats changes
        
        if (Target == "Player") //if it's a player, pull from player model
            stats = BattleManager.Instance.Player.ActiveStats;
        else if (Target == "Enemy") //vice versa for enemy
            stats = BattleManager.Instance.CurrentEnemy.ActiveStats;
        else //if none then ditch it again
            return;
        
        int value = 0; //use this to store the value of stat changes on each type
        Stat currentStat; //use this to 
        TMP_Text statNumber = null;
        
        //Attack
        if (stats.TryGetValue(StatType.AttackUP, out currentStat))
        {
            value += currentStat.Value;
        }
        if (stats.TryGetValue(StatType.AttackDOWN, out currentStat))
        {
            value -= currentStat.Value;
        }
        
        //attack changes
        if (value != 0)
        {
            if (value < 0)
            {
                statNumber = attackDown.transform.GetComponentInChildren<TMP_Text>();
                if (!attackDown.activeSelf)
                {   
                    attackDown.SetActive(true);
                    statNumber.text = value.ToString();
                }
                else statNumber.text = value.ToString();

                if (attackUp.activeSelf) attackUp.SetActive(false);
            }
            else if (value > 0)
            {
                statNumber = attackUp.transform.GetComponentInChildren<TMP_Text>();
                if (!attackUp.activeSelf)
                {   
                    attackUp.SetActive(true);
                    statNumber.text = value.ToString();
                }
                else statNumber.text = value.ToString();
                
                if (attackDown.activeSelf) attackDown.SetActive(false);
            }
            value = 0;
        }
        else
        {
            if (attackUp.activeSelf) attackUp.SetActive(false);
            if (attackDown.activeSelf) attackDown.SetActive(false);
        }
        
        //def
        if (stats.TryGetValue(StatType.DefenseUP, out currentStat))
        {
            value += currentStat.Value;
        }
        if (stats.TryGetValue(StatType.DefenseDOWN, out currentStat))
        {
            value -= currentStat.Value;
        }
        
        //def changes
        if (value != 0)
        {
            if (value < 0)
            {
                statNumber = defDown.transform.GetComponentInChildren<TMP_Text>();
                if (!defDown.activeSelf)
                {   
                    defDown.SetActive(true);
                    statNumber.text = value.ToString();
                }
                else statNumber.text = value.ToString();

                if (defUp.activeSelf) defUp.SetActive(false);
            }
            else if (value > 0)
            {
                statNumber = defUp.transform.GetComponentInChildren<TMP_Text>();
                if (!defUp.activeSelf)
                {   
                    defUp.SetActive(true);
                    statNumber.text = value.ToString();
                }
                else statNumber.text = value.ToString();
                
                if (defDown.activeSelf) defDown.SetActive(false);
            }
            value = 0;
        }
        else
        {
            if (defUp.activeSelf) defUp.SetActive(false);
            if (defDown.activeSelf) defDown.SetActive(false);
        }
        
        //move
        if (stats.TryGetValue(StatType.MovesUP, out currentStat))
        {
            value += currentStat.Value;
        }
        if (stats.TryGetValue(StatType.MovesDOWN, out currentStat))
        {
            value -= currentStat.Value;
        }
        
        //move changes
        if (value != 0)
        {
            if (value < 0)
            {
                statNumber = moveDown.transform.GetComponentInChildren<TMP_Text>();
                if (!moveDown.activeSelf)
                {   
                    moveDown.SetActive(true);
                    statNumber.text = value.ToString();
                }
                else statNumber.text = value.ToString();

                if (moveDown.activeSelf) moveUp.SetActive(false);
            }
            else if (value > 0)
            {
                statNumber = moveUp.transform.GetComponentInChildren<TMP_Text>();
                if (!moveUp.activeSelf)
                {   
                    moveUp.SetActive(true);
                    statNumber.text = value.ToString();
                }
                else statNumber.text = value.ToString();
                
                if (moveDown.activeSelf) moveDown.SetActive(false);
            }
            value = 0;
        }
        else
        {
            if (moveUp.activeSelf) moveUp.SetActive(false);
            if (moveDown.activeSelf) moveDown.SetActive(false);
        }
    }
    
    public void ShowPreview()
    {
        if(Target == "")
            return;
        
        string displayStr = "";

        Dictionary<StatType, Stat> stats;
        
        if (Target == "Player")
            stats = BattleManager.Instance.Player.ActiveStats;
        else if (Target == "Enemy")
            stats = BattleManager.Instance.CurrentEnemy.ActiveStats;
        else
            return;
        
        int value = 0;
        Stat currentStat;
        
        //Attack
        if (stats.TryGetValue(StatType.AttackUP, out currentStat))
        {
            value += currentStat.Value;
        }
        if (stats.TryGetValue(StatType.AttackDOWN, out currentStat))
        {
            value -= currentStat.Value;
        }

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
        if (stats.TryGetValue(StatType.DefenseUP, out currentStat))
        {
            value += currentStat.Value;
        }
        if (stats.TryGetValue(StatType.DefenseDOWN, out currentStat))
        {
            value -= currentStat.Value;
        }

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
        if (stats.TryGetValue(StatType.MovesUP, out currentStat))
        {
            value += currentStat.Value;
        }
        if (stats.TryGetValue(StatType.MovesDOWN, out currentStat))
        {
            value -= currentStat.Value;
        }

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
}
