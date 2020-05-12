using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance = null;

    private Transform _map;

    private Transform _tracks;
    //Map Directory
    public Dictionary<string, DirectoryEntry> MapDirectory;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if(Instance != this)
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!File.Exists(Application.persistentDataPath + "/mapdata.save"))
        {
            SaveMap();
        }

        _map = GameObject.Find("Map").transform;
        _tracks = GameObject.FindWithTag("OverworldTracks").transform;
        LoadDirectory();
        LoadMap();
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
    
    public void SaveMap()
    {
        MapData md = new MapData();
        
        //Player loc
        md.PlayerNodeID = GameObject.FindWithTag("OverworldTrain").GetComponent<OverworldTrain>().CurrentNode.NodeID;
        
        //Enemies & Interactables
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("OverworldInteractable"))
        {
            var interactable = go.GetComponent<MapInteractable>();
            
            if (interactable.GetType() == typeof(EnemyMapObstacle))
            {
                var obstacle = go.GetComponent<EnemyMapObstacle>();
                md.Enemies.Add(new EnemyNodeInfo(obstacle.Node.NodeID, obstacle.MyScene));
            }
            else
            {
                md.InteractableLocations.Add(interactable.Node.NodeID);
            }
        }
        
        //Save!
        Utils.Save(md, "mapdata");
    }

    private void LoadMap()
    {
        MapData md = Utils.Load<MapData>("mapdata");

        OverworldTrain.Instance.CurrentNode = _tracks
            .Find(md.PlayerNodeID.Substring(0, 1)).transform.Find(md.PlayerNodeID.Substring(1, 1))
            .GetComponent<MapNode>();
        OverworldTrain.Instance.SnapToNode();

        //Create
        foreach (var e in md.Enemies)
        {
            if (!MapDirectory.ContainsKey(e.NodeID))
            {
                EnemyMapObstacle newEnemy = Instantiate(Resources.Load<GameObject>("Prefabs/Map/EnemyInteractable"), _map)
                    .GetComponent<EnemyMapObstacle>();
                newEnemy.Node = _tracks
                    .Find(e.NodeID.Substring(0, 1)).transform.Find(e.NodeID.Substring(1, 1))
                    .GetComponent<MapNode>();
                newEnemy.MyScene = e.SceneName;
                MapDirectory.Add(e.NodeID, new DirectoryEntry(newEnemy.gameObject, true));
            }
        }
        
        foreach (var i in md.InteractableLocations)
        {
            if (!MapDirectory.ContainsKey(i))
            {
                MapInteractable newInteractable = Instantiate(Resources.Load<GameObject>("Prefabs/Map/Interactable"), _map)
                    .GetComponent<MapInteractable>();
                newInteractable.Node = _tracks
                    .Find(i.Substring(0, 1)).transform.Find(i.Substring(1, 1))
                    .GetComponent<MapNode>();
                MapDirectory.Add(i, new DirectoryEntry(newInteractable.gameObject, false));
            }
        }
        
        //Destroy!
        foreach (var d in MapDirectory)
        {
            if(!d.Value.IsEnemy)
                continue;
            bool found = false;
            foreach (var e in md.Enemies)
            {
                if (e.NodeID == d.Key)
                    found = true;
            }
            
            if(!found)
                Destroy(d.Value.gameObject);
        }
    }

    private void LoadDirectory()
    {
        MapDirectory = new Dictionary<string, DirectoryEntry>();
        //Enemies & Interactables
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("OverworldInteractable"))
        {
            var interactable = go.GetComponent<MapInteractable>();
            
            MapDirectory.Add(interactable.Node.NodeID, new DirectoryEntry(go, interactable.GetType() == typeof(EnemyMapObstacle)));
        }
    }
}

public struct DirectoryEntry
{
    public GameObject gameObject;
    public bool IsEnemy;

    public DirectoryEntry(GameObject go, bool isEnemy)
    {
        gameObject = go;
        IsEnemy = isEnemy;
    }
}
