using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyMapObstacle : MapObstacle
{
    public string MyScene;

    private bool _triggered;
    
    private void OnTriggerStay2D(Collider2D other)
    {
        OnTriggerEnter2D(other);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("OverworldTrain"))
            return;
        if(OverworldTrain.Instance.NextNode != Node)
            return;
        if(_triggered)
            return;
        _triggered = true;
        
        if (OverworldTrain.Instance.CurrentNode == Node)
        {
            Destroy(gameObject);
            return;
        }

        Utils.Save(new PlayerMapData(Node.NodeID), "playermapdata");
        GameManager.Instance.LoadScene(MyScene);
        GameManager.Instance.State = GameState.Battle;
    }

}
