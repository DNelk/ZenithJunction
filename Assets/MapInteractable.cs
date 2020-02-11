using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        OverworldTrain.Instance.TravelToNode(Node);
    }
}
