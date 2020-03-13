using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardDirectoryData
{
    public List<CardEntry> CardsByName;
    public List<CardEntry> BigEcon;
    public List<CardEntry> Melee;
    public List<CardEntry> Ranged;

    public CardDirectoryData()
    {
        CardsByName = new List<CardEntry>();
        BigEcon = new List<CardEntry>();
        Melee = new List<CardEntry>();
        Ranged = new List<CardEntry>();
    }
}

[Serializable]
public struct CardEntry
{
    public string Name;
    public string Value;

    public CardEntry(string name, string value)
    {
        Name = name;
        Value = value;
    }
}