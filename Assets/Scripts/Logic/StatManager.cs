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
}
