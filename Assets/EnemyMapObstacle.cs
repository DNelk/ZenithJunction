using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMapObstacle : MapObstacle
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("OverworldTrain") || OverworldTrain.Instance.CurrentNode != Node)
            return;
        
        
    }
}
