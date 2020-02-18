using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyMapObstacle : MapObstacle
{
    public SceneAsset MyScene;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("OverworldTrain"))
            return;
        if (OverworldTrain.Instance.CurrentNode == Node)
        {
            Destroy(gameObject);
            return;
        }

        Utils.Save(new PlayerMapData(Node.NodeID), "playermapdata");
        GameManager.Instance.LoadScene(MyScene.name);
        GameManager.Instance.State = GameState.Battle;
    }

}
