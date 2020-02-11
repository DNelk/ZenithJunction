using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!File.Exists(Application.persistentDataPath + "playermapdata.save"))
        {
            PlayerMapData pmd = new PlayerMapData(OverworldTrain.Instance.CurrentNode.NodeID);
            Utils.Save(pmd, "playermapdata");
        }

        PlayerMapData mapData = Utils.Load<PlayerMapData>("playermapdata");

        OverworldTrain.Instance.CurrentNode = GameObject.FindWithTag("OverworldTracks").transform
            .Find(mapData.NodeID.Substring(0, 1)).transform.Find(mapData.NodeID.Substring(1, 1))
            .GetComponent<MapNode>();
        OverworldTrain.Instance.SnapToNode();
    }

    // Update is called once per frame
    void Update()
    {
        if (OverworldTrain.Instance.NextNode == null)
        {
            //On Up, move to next one if its not a choice
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (!OverworldTrain.Instance.CurrentNode.IsFork)
                {
                    OverworldTrain.Instance.TravelToNext();
                }
            }

            //On down, move to previous one
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                OverworldTrain.Instance.TravelToPrevious();
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                OverworldTrain.Instance.Fork(true);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                OverworldTrain.Instance.Fork(false);
            }
        }
    }
}
