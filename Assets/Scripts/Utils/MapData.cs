using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapData
{
    public string PlayerNodeID; //Player's Location
    public List<EnemyNodeInfo> Enemies; //Enemies and their locations
    public List<string> InteractableLocations; //Generic interactable locations

    public MapData()
    {
        PlayerNodeID = "";
        Enemies = new List<EnemyNodeInfo>();
        InteractableLocations = new List<string>();
    }
}
[Serializable]
public struct EnemyNodeInfo
{
    public string NodeID;
    public string SceneName;

    public EnemyNodeInfo(string id, string scene)
    {
        NodeID = id;
        SceneName = scene;
    }
}