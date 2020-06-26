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

    //Call whenever checking a value against stats
    //This will run through the stat list and compare against the values therein, returning a modified int
    //The uptype will INCREASE the modifiedValue, while the downtype DECREASES the value
    public int StatCheck(int input, List<Stat> statsList, StatType upType, StatType downType)
    {
        int modifiedValue = input;
        foreach (var stat in statsList)
        {
            if (!stat.IsNew)
            {
                if (stat.StatType == upType)
                    modifiedValue += stat.Value;
                else if (stat.StatType == downType)
                    modifiedValue -= stat.Value;
            }
        }
        return modifiedValue;
    }

    //Called whenever you need to remove all stat changes
    public void ClearStats(List<Stat> statsList)
    {
        statsList.Clear();        
    }
}
