using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
//Script for nodes on the overworld map
public class MapNode : MonoBehaviour
{
    public MapNode Previous; //Previous node on our track
    public MapNode[] Next; //Next node on our track
    public bool IsFork; //Does this node fork (are there multiple possible next nodes)
    public string NodeID => transform.parent.name + gameObject.name;

    public bool HasObstacle()
    {
        GameObject[] interactables = GameObject.FindGameObjectsWithTag("OverworldInteractable");
        foreach (var i in interactables)
        {
            MapInteractable interactable = i.GetComponent<MapInteractable>();
            //Is it ours?
            if(interactable.Node.NodeID != NodeID)
                continue;
            //Is it an obstacle?
            //Do we have to stop here?
            if (!interactable.AllowPass)
            {
                return true;
            }
        }
        return false;
    }
}
