using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is called by Player and Enemy when they need to add new stats or tick current stats down
/// </summary>
public class StatManager : MonoBehaviour
{
    public static StatManager Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if(Instance != this)
            Destroy(gameObject);
    }
    
    //Called by cards that add stat values
    public void ModifyStat(List<Stat> statsList, StatType type, int turnsLeft, int value, HealthBar healthBar, bool applyImmidiately = false)
    {
        statsList.Add(new Stat(turnsLeft, value, applyImmidiately, type));
        healthBar.UpdateStatusChanges();
    }
    
    //Called in the battle manager for both the player and enemy to count down remaining turns of a stat
    public void TickDownStats(List<Stat> statsList, HealthBar healthBar)
    {
        foreach (var stat in statsList)
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
                statsList.Remove(stat);
            }
        }
        
        healthBar.UpdateStatusChanges();
    }

    //Called from Enemy and Player when they take damage
    //Run through the list and see if there are relevant defense values
    public int DefenseStatCheck(int damage, List<Stat> statsList)
    {
        int modifiedDamage = damage;

        foreach (var stat in statsList)
        {
            if (!stat.IsNew)
            {
                if (stat.StatType == StatType.DefenseUP)
                    modifiedDamage -= stat.Value;
                else if (stat.StatType == StatType.DefenseDOWN)
                    modifiedDamage += stat.Value;
            }
        }

        return modifiedDamage;
    }
}
