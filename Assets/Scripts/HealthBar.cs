using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public string Target = "";
    public Tooltip _tooltip;
    
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
