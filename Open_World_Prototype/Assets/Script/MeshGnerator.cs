using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MeshGnerator : MonoBehaviour
{
    Mesh mesh;
    MeshCollider meshCollider;
    GameObject newMesh;

    Dictionary<Vector2, Vector3[]> myMeshes = new Dictionary<Vector2, Vector3[]>();
    Vector3[] vertices;
    int[] triangles;

    public int xSize;
    public int zSize;

    private float centerX = 0;
    private float centerZ = 0;

    public int chunks;

    public Material snow;

    private void Awake()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        CreateMesh(0,0);
        ChunkGenerator();

    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateShape()
    {
        Debug.Log("Is In CreateShape" + newMesh.transform.position.x + "," + newMesh.transform.position.z);
        Array.Clear(vertices, 0, vertices.Length);
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];


        int xMin = (xSize / 2) * -1;
        int xMax = xSize / 2;

        int zMin = (zSize / 2) * -1;
        int zMax = zSize / 2;

        float newMeshX = newMesh.transform.position.x;
        float newMeshZ = newMesh.transform.position.z;
        bool leftMesh = myMeshes.ContainsKey(new Vector2(newMeshX - xSize, newMeshZ));
        bool topMesh = myMeshes.ContainsKey(new Vector2(newMeshX, newMeshZ + zSize));
        bool rightMesh = myMeshes.ContainsKey(new Vector2(newMeshX + xSize, newMeshZ));
        bool bottomMesh = myMeshes.ContainsKey(new Vector2(newMeshX, newMeshZ - zSize));
        bool topLeftMesh = myMeshes.ContainsKey(new Vector2(newMeshX - xSize, newMeshZ + zSize));
        bool topRightMesh = myMeshes.ContainsKey(new Vector2(newMeshX + xSize, newMeshZ + zSize));
        bool bottomRightMesh = myMeshes.ContainsKey(new Vector2(newMeshX + xSize, newMeshZ - zSize));
        bool bottomLeftMesh = myMeshes.ContainsKey(new Vector2(newMeshX - xSize, newMeshZ - zSize));

        Debug.Log("enter for loop vertices in CreateShape" + newMesh.transform.position.x + "," + newMesh.transform.position.z);

        for (int i = 0, z = zMin; z <= zMax; z++)
        {
            for (int x = xMin; x <= xMax; x++)
            {
                float y = Mathf.PerlinNoise(x * .3f, z * .3f) * 2f;
                if (x == xMin && leftMesh)
                {
                    y = myMeshes[new Vector2(newMeshX - xSize, newMeshZ)][i].y;
                }
                if(z == zMax && topMesh)
                {
                    y = myMeshes[new Vector2(newMeshX, newMeshZ + zSize)][i].y;
                }
                if(x == xMax && rightMesh)
                {
                    y = myMeshes[new Vector2(newMeshX + xSize, newMeshZ)][i].y;
                }
                if(z == zMin && bottomMesh)
                {
                    y = myMeshes[new Vector2(newMeshX, newMeshZ - zSize)][i].y;
                }
                if(x == xMin && z == zMax && !leftMesh && !topMesh && topLeftMesh)
                {
                    y = myMeshes[new Vector2(newMeshX - xSize, newMeshZ + zSize)][i].y;
                }
                if (x == xMax && z == zMax && !rightMesh && !topMesh && topRightMesh)
                {
                    y = myMeshes[new Vector2(newMeshX + xSize, newMeshZ + zSize)][i].y;
                }
                if (x == xMax && z == zMin && !rightMesh && !bottomMesh && bottomRightMesh)
                {
                    y = myMeshes[new Vector2(newMeshX + xSize, newMeshZ - zSize)][i].y;
                }
                if (x == xMin && z == zMin && !rightMesh && !bottomMesh && bottomLeftMesh)
                {
                    y = myMeshes[new Vector2(newMeshX - xSize, newMeshZ - zSize)][i].y;
                }
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        Debug.Log("Out for loop vertices in CreateShape" + newMesh.transform.position.x + "," + newMesh.transform.position.z);
        myMeshes.Add(new Vector2(newMesh.transform.position.x, newMesh.transform.position.z), vertices);
        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
        Debug.Log("Out for loop triangle in CreateShape" + newMesh.transform.position.x + "," + newMesh.transform.position.z);
    }

    void CreateMesh(float x, float z)
    {
        Debug.Log("enter CreateMesh" + x + "," + z);
        newMesh = new GameObject("Mesh" + x + "," + z);
        newMesh.transform.position = new Vector3(x, 0, z);
        newMesh.transform.parent = transform;
        newMesh.AddComponent<MeshFilter>();
        newMesh.AddComponent<MeshRenderer>();
        mesh = new Mesh();
        newMesh.GetComponent<MeshFilter>().mesh = mesh;
        newMesh.GetComponent<MeshRenderer>().material = snow;
        Debug.Log("Go In CreateShape" + x + "," + z);
        CreateShape();
        UpdateMesh();
        Debug.Log("out updatemesh" + newMesh.transform.position.x + "," + newMesh.transform.position.z);
        newMesh.AddComponent<MeshCollider>();
        newMesh.GetComponent<MeshCollider>().sharedMesh = mesh;
        Debug.Log("out CreateMesh" + newMesh.transform.position.x + "," + newMesh.transform.position.z);

    }

    void ChunkGenerator()
    {
        Debug.Log("Enter chunkGenrator");

        for (float x = centerX - (xSize * chunks); x <= (xSize * chunks); x+=xSize)
        {
            for (float z = centerZ - (zSize * chunks); z <= (zSize * chunks); z+=zSize)
            {
                Debug.Log("in loop with chunkGenrator");
                if (x != centerX || z != centerZ)
                {
                    Debug.Log("Enter createMesh with chunkGenrator");
                    CreateMesh(x,z);

                }
            }
        }
    }

    void UpdateMesh()
    {

        Debug.Log("enter updatemesh" + newMesh.transform.position.x + "," + newMesh.transform.position.z);
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
