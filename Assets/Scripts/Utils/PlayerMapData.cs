using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerMapData
{
    public string NodeID;

    public PlayerMapData(string id)
    {
        NodeID = id;
    }
}
