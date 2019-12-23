using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    #region Private Variables
    private MeshFilter myMF; //Our Mesh filter
    private MeshRenderer myMR; //Our Mesh renderer
    private MeshCollider myMC; //Our Mesh Collider
    private Rigidbody myRB; //Our Rigid Body
    private Mesh myMesh; //Our Mesh

    private Vector3[] verts; //Our vertices
    private int[] tris; //Our Tris
    private Vector2[] uVs; //Our UVs
    private Vector3[] normals; //Our Normals

    //Indices of vertices and tris
    private int totalVertInd;
    private int totalTrisInd;

    private bool onTrack;
    private List<GameObject> trees;
    private int treeCount = 3;
    #endregion
    
    public static int SizeSquare = 100; //Square root of amount of quads of our chunk 
    
    private void Awake()
    {
        myMF = gameObject.AddComponent<MeshFilter>();
        myMR = gameObject.AddComponent<MeshRenderer>();
        myMC = gameObject.AddComponent<MeshCollider>();
        myRB = gameObject.AddComponent<Rigidbody>();
        myMesh = new Mesh();
    }

    private void Start()
    {
        Init();
        CalcMesh();
        ApplyMesh();
    }

    //Figure out how many things we need and make the variables
    private void Init()
    {
        totalVertInd = (SizeSquare + 1) * (SizeSquare + 1); 
        totalTrisInd = (SizeSquare) * (SizeSquare) * 2 * 3;
        
        verts = new Vector3[totalVertInd];
        tris = new int[totalTrisInd];
        uVs = new Vector2[totalVertInd];
        normals = new Vector3[totalVertInd];

        myRB.isKinematic = true;

        onTrack = false;
    }
      
    private void CalcMesh()
    {
        //Assign our vertices UVs, giving a y based on transform position and noise
        for (int z = 0; z <= SizeSquare; z++)
        {
            for (int x = 0; x <= SizeSquare; x++)
            {
                verts[z * (SizeSquare + 1) + x] = new Vector3(x
                   ,100*Perlin.Noise((transform.position.x + (float)x)/SizeSquare , (transform.position.z + (float)z)/SizeSquare), z);
                uVs[z * (SizeSquare + 1) + x] = new Vector2((x) / (float) SizeSquare, (z) / (float) SizeSquare);
            }
        }
        
        CalcTris();
    }

    //Apply our vertices and such to the mesh
    private void ApplyMesh()
    {
        myMesh.vertices = verts;
        myMesh.triangles = tris;
        myMesh.uv = uVs;
        myMesh.RecalculateNormals();
        
        myMF.mesh = myMesh;
        
        ApplyMaterial();

        myMC.sharedMesh = myMesh;
        /*if(trees == null && !onTrack)
            SpawnStuff();*/
    }

    public void RecalculateMesh()
    {
        CalcMesh();
        ApplyMesh();
       /* if(!onTrack) 
            UpdateTrees();*/
    }

    //Calculate our tris
    private void CalcTris()
    {
        //Assign our tris
        int triInd = 0;
        int bottomLeft, topLeft, bottomRight, topRight;      
        for (int j = 0; j < SizeSquare; j++)
        {
            for (int k = 0; k < SizeSquare; k++)
            {
                bottomLeft = k + (j * (SizeSquare + 1));
                topLeft = k + ((j + 1) *(SizeSquare + 1));
                bottomRight = bottomLeft + 1;
                topRight = topLeft + 1;

                tris[triInd] = bottomLeft;
                triInd++;
                tris[triInd] = topLeft;
                triInd++;
                tris[triInd] = bottomRight;
                triInd++;
                tris[triInd] = topLeft;
                triInd++;
                tris[triInd] = topRight;
                triInd++;
                tris[triInd] = bottomRight;
                triInd++;
            }
        }
    }

    private void ApplyMaterial()
    {
        if (!onTrack)
        {
            myMR.material = Resources.Load<Material>("materials/terrainMat");
        }
        else
        {
            myMR.material = Resources.Load<Material>("materials/terrainMat");
        }
    } 

    public bool OnTrack
    {
        get { return onTrack; }
        set { onTrack = value; ApplyMaterial(); SpawnStuff();}
    }

    //Create Bumpers if we're on track or trees if we're not
    private void SpawnStuff()
    {
        if (!onTrack)
        {
            SpawnTrees();
        }
    }

    private void DestroyTrees()
    {
        if (trees != null)
        {
            foreach (GameObject tree in trees)
            {
                Destroy(tree);
            }

            trees.Clear();
        }
    }
    private void SpawnTrees()
    {
        if (trees == null)
            trees = new List<GameObject>();
        for (int i = 0; i < treeCount; i++)
        {
            trees.Add(Instantiate(Resources.Load<GameObject>("prefabs/pine")));
            trees[i].transform.SetParent(GameObject.FindWithTag("Trees").transform);
        }
        UpdateTrees();
    }

    public void UpdateTrees()
    {
        for (int i = 0; i < trees.Count; i++)
        {
            trees[i].transform.position = transform.position + (Vector3.right * Random.Range(0, SizeSquare / 2))
                                                             + (Vector3.forward * Random.Range(0, SizeSquare / 2));
            trees[i].transform.localScale += Vector3.up * Random.Range(0f, 4f);
        }
    }
    
}