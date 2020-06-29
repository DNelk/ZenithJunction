
using System;

[Serializable]
public class Stat
{
    public int TurnsLeft;
    public int Value;
    public bool IsNew;
    public StatType StatType;

    public Stat(int turnsLeft, int value)
    {
        TurnsLeft = turnsLeft;
        Value = value;
        IsNew = true;
    }
    
    public Stat(int turnsLeft, int value, bool applyImmediately)
    {
        TurnsLeft = turnsLeft;
        Value = value;
        if (applyImmediately)
            IsNew = false;
        else
            IsNew = true;
    }
    
    public Stat(int turnsLeft, int value, bool applyImmediately, StatType statType)
    {
        TurnsLeft = turnsLeft;
        Value = value;
        if (applyImmediately)
            IsNew = false;
        else
            IsNew = true;
        StatType = statType;
    }
}

public enum StatType
{
    DefenseUP,
    AttackUP,
    MovesUP,
    DefenseDOWN,
    AttackDOWN,
    MovesDOWN,
    NullStat
}