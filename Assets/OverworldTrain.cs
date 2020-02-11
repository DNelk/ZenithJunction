using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class OverworldTrain : MonoBehaviour
{
    //Node Info
    public MapNode CurrentNode;
    public MapNode NextNode;

    //Tweening
    private Sequence _travelTween;
    
    //Instance
    public static OverworldTrain Instance = null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if(Instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        SnapToNode();
    }

    public void SnapToNode()
    {
        transform.position = CurrentNode.transform.position;
    }
    
    public void TravelToNext(int index = 0)
    {
        if(CurrentNode.Next == null || CurrentNode.Next.Length < 1)
            return;
        NextNode = CurrentNode.Next[index];
        Travel();
    }
    
    public void TravelToPrevious()
    {
        if(CurrentNode.Previous == null)
            return; 
        NextNode = CurrentNode.Previous;
        Travel();
    }

    public void Fork(bool left)
    {
        if (CurrentNode.IsFork)
        {
            if (CurrentNode.Next.Length < 1)
                return;
            if (CurrentNode.Next.Length < 2 || left)
                TravelToNext(0);
            else
                TravelToNext(1);
        }
        else
        {
            if (left)
            {
                if (CurrentNode.Previous != null && CurrentNode.Previous.IsFork &&
                    CurrentNode.Previous.Next[1] == CurrentNode)
                    TravelToPrevious();
                else
                    TravelToNext();
            }
            else
            {
                if (CurrentNode.Previous != null && CurrentNode.Previous.IsFork &&
                    CurrentNode.Previous.Next[0] == CurrentNode)
                    TravelToPrevious();
                else
                    TravelToNext();
            }
        }
    }

    //Finds paths!
    public void TravelToNode(MapNode node)
    {
        StartCoroutine(TravelToNode(node.NodeID));
    }
    
    public IEnumerator TravelToNode(string nodeID)
    {
        List<string> path = new List<string>();
        
        //Find the shortest path from desired node to our current node
        if (GetPath(CurrentNode, path, nodeID))
            Debug.Log("path found forward");
        else if (GetPath(CurrentNode, path, nodeID, false))
            Debug.Log("path found backward");
        else
            yield break;

        for (int i = 0; i < path.Count - 1; i++)
        {
            if (CurrentNode.HasObstacle())
                continue;
            
            string dir = path[i];

            if (dir == "up")
                TravelToNext();
            else if (dir == "down")
                TravelToPrevious();
            else if (dir == "fork")
                TravelToNext(1);
            else
                continue;

            yield return _travelTween.WaitForCompletion();
        }
    }

    private void Travel()
    {
        if(_travelTween != null)
            return;
        
        Vector3 rotDir = NextNode.transform.position - transform.position;
        float angle = Mathf.Atan2(rotDir.y, rotDir.x) * Mathf.Rad2Deg;
        
        _travelTween = DOTween.Sequence();
        _travelTween.Append(transform.DOMove(NextNode.transform.position, .5f));
        _travelTween.Join(transform.DORotate(Quaternion.AngleAxis(angle, Vector3.forward).eulerAngles,0.5f));
        _travelTween.OnComplete(TravelDone);
    }

    private void TravelDone()
    {
        _travelTween = null;
        CurrentNode = NextNode;
        NextNode = null;
    }
    
    //Checks if there is a path between two nodes and creates a list of points
    private bool GetPath(MapNode root, List<string> path, string nodeID, bool forward = true)
    {
        //See if the root actually exists lol
        if (root == null)
            return false;
        
        //First stop... the root
        path.Add("up");
        int myIndex = path.Count - 1;
        
        //Did we make it?
        if (root.NodeID == nodeID)
            return true;

        //Recursion time! Run the function on the current node's neighbors
        if (forward)
        {
            if (root.Next.Length > 0)
            {
                if (GetPath(root.Next[0], path, nodeID))
                    return true;
                if (root.IsFork && GetPath(root.Next[1], path, nodeID))
                {
                    path[myIndex] = "fork";
                    return true;
                }
            }
        }
        else
        {
            path[myIndex] = "down";
            
            if (GetPath(root.Previous, path, nodeID, false))
                return true;
        
            //We might be able to get there by forking
            if (root.IsFork)
            {
                if (GetPath(root.Next[0], path, nodeID))
                {
                    path[myIndex] = "up";
                    return true;
                }

                if (GetPath(root.Next[1], path, nodeID))
                {
                    path[myIndex] = "fork";
                    return true;
                }
            }
        }

        //Ok so if we're here, that means we didnt find a path to the desired node.
        //Let's delete this current node from the list
        path.RemoveAt(myIndex);
        return false;
    }
}
