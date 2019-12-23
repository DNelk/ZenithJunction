using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    #region Public Variables
   
    //public List<Chunk> Chunks; //List of all our chunks
    public Chunk[,] Chunks; //2D array of all our chunks

    public Transform View; //View

    public int ChunksX = 20; //How many chunks to make, squared
    public int ChunksY = 20;
    
    public Transform Target; // Our target for treadmilling
    #endregion

    #region Private Variables

    //Used to move our chunks based on our own position, converted to integers
    private Vector2 lastChunkInd;
    private Vector2 currChunkInd;
    private Vector2 currChunkPos;
    #endregion
    
    //Populate our list of chunks
    void Start()
    {
       //Chunks = new List<Chunk>();
        Chunks = new Chunk[ChunksX, ChunksY];

        for (int j = 0; j < ChunksX; j++)
        {
            for (int k = 0; k < ChunksY; k++)
            {
                Vector3 origin = new Vector3(j * Chunk.SizeSquare, 0, k * Chunk.SizeSquare);
                //Instantiate a chunk prefab and add its chunk component to our list
                Chunks[j,k] = Instantiate(Resources.Load<GameObject>("prefabs/ProceduralTerrain/Chunk"), View.TransformPoint(origin), Quaternion.identity, View).GetComponent<Chunk>();
            }
        }
        
        CalcChunkInd();
        lastChunkInd = currChunkInd;
    }

    // Update is called once per frame
    void Update()
    {
        if (Chunks.GetLength(0) == 0)
            return;
        //Check which Chunk we're on
        CalcChunkInd();
        //See if we moved and our old index if so
        if (currChunkInd != lastChunkInd)
        {
            if (currChunkInd.x > lastChunkInd.x) // We moved left
            {
                Debug.Log("moved left");
                Chunk[] tempChunks = new Chunk[Chunks.GetLength(1)];

                for (int j = 0; j < Chunks.GetLength(0); j++)
                {
                    for (int k = 0; k < Chunks.GetLength(1); k++)
                    {
                        if (j == 0) //We only want to move the last column
                        {
                            tempChunks[k] = Chunks[j, k];
                        }
                        else
                        {
                            Chunks[j - 1, k] = Chunks[j, k];
                            if (j == Chunks.GetLength(0) - 1) //Set the very last row to the temp row
                            {
                                Chunks[j, k] = tempChunks[k];
                                Chunks[j, k].transform.position =
                                    Chunks[j - 1, k].transform.position + (Vector3.right * Chunk.SizeSquare);
                                Chunks[j, k].RecalculateMesh();
                            }
                        }
                    }
                }
            }
            
            if (currChunkInd.x < lastChunkInd.x) // We moved right
            {
                Debug.Log("moved right");
                Chunk[] tempChunks = new Chunk[Chunks.GetLength(1)];
                
                //Go through in reverse so we can set values lower on the array
                for (int j = Chunks.GetLength(0) - 1; j >= 0; j--)
                {
                    for (int k = 0; k < Chunks.GetLength(1); k++)
                    {
                        if (j == Chunks.GetLength(0) - 1) //We only want to move the last column
                        {
                            tempChunks[k] = Chunks[j, k];
                        }
                        else
                        {
                            Chunks[j + 1, k] = Chunks[j, k];
                            if (j == 0) //Set the very last row to the temp row
                            {
                                Chunks[j, k] = tempChunks[k];
                                Chunks[j, k].transform.position = 
                                    Chunks[j + 1, k].transform.position - (Vector3.right * Chunk.SizeSquare);
                                Chunks[j, k].RecalculateMesh();
                            }
                        }
                    }
                }
            }

            if (currChunkInd.y > lastChunkInd.y) // We moved back
            {
                Debug.Log("moved back");
                Chunk[] tempChunks = new Chunk[Chunks.GetLength(0)];

                for (int k = 0; k < Chunks.GetLength(1); k++)
                {
                    for (int j = 0; j < Chunks.GetLength(0); j++)
                    {
                        if (k == 0) //We only want to move the last row
                        {
                            tempChunks[j] = Chunks[j, k];
                        }
                        else
                        {
                            Chunks[j, k - 1] = Chunks[j, k];
                            if (k == Chunks.GetLength(1) - 1) //Set the very last row to the temp row
                            {
                                Chunks[j, k] = tempChunks[j];
                                Chunks[j, k].transform.position =
                                    Chunks[j, k - 1].transform.position + (Vector3.forward * Chunk.SizeSquare);
                                Chunks[j, k].RecalculateMesh();
                            }
                        }
                    }
                }
            }

            if (currChunkInd.y < lastChunkInd.y) // We moved forward
            {
                Debug.Log("moved forward");
                Chunk[] tempChunks = new Chunk[Chunks.GetLength(0)];
                
                //Go through in reverse so we can set values lower on the array
                for (int k = Chunks.GetLength(0) - 1; k >= 0; k--)
                {
                    for (int j = 0; j < Chunks.GetLength(0); j++)
                    {
                        if (k == Chunks.GetLength(0) - 1) //We only want to move the last column
                        {
                            tempChunks[j] = Chunks[j, k];
                        }
                        else
                        {
                            Chunks[j, k + 1] = Chunks[j, k];
                            if (k == 0) //Set the very last row to the temp row
                            {
                                Chunks[j, k] = tempChunks[j];
                                Chunks[j, k].transform.position = 
                                    Chunks[j, k + 1].transform.position - (Vector3.forward * Chunk.SizeSquare);
                                Chunks[j, k].RecalculateMesh();
                            }
                        }
                    }
                }
            }
            
            CalcChunkInd(); //Double-check our position since we shifted the array
            lastChunkInd = currChunkInd;
        }
    }

    //Find out what chunk we're on
    private void CalcChunkInd()
    {
        //Find out what our target's chunk coordinate is
        currChunkPos = new Vector2(Mathf.Floor(Target.position.x/Chunk.SizeSquare), Mathf.Floor(Target.position.z/Chunk.SizeSquare));
        
        for(int j = 0; j < Chunks.GetLength(0); j++)
        {
            for (int k = 0; k < Chunks.GetLength(1); k++)
            {
                if (Mathf.Floor(Chunks[j, k].transform.position.x / Chunk.SizeSquare) == currChunkPos.x &&
                    Mathf.Floor(Chunks[j, k].transform.position.z / Chunk.SizeSquare) == currChunkPos.y)
                {
                    //Debug.Log("Current Chunk: " + j + "," + k);
                    currChunkInd = new Vector2(j,k);
                }
            }
        }
        
        for (int i = 0; i < Chunks.GetLength(1); i++)
        {
            Chunks[(int) currChunkInd.x, i].OnTrack = true;
        }
    }
}