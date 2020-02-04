using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInteractable : MonoBehaviour
{
    public MapNode Node;
    public bool AllowPass;
    private void Start()
    {
        SnapToNode();
    }

    private void SnapToNode()
    {
        transform.position = Node.transform.position;
    }

    private void OnMouseDown()
    {
        OverworldTrain.Instance.TravelToNode(Node);
    }
}
