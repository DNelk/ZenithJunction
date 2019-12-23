public class Stat
{
    public int TurnsLeft;
    public int Value;
    public bool IsNew;

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
}

public enum StatType
{
    DefenseUP,
    AttackUP,
    MovesUP,
    DefenseDOWN,
    AttackDOWN,
    MovesDOWN
}