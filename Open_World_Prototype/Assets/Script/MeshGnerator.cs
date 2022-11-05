using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MeshGnerator : MonoBehaviour
{
    Mesh mesh;
    MeshCollider meshCollider;
    GameObject newMesh;

    Vector3[] vertices;
    int[] triangles;

    public int xSize;
    public int zSize;

    public Material snow;

    private void Awake()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        CreateMesh();

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
        Array.Clear(vertices, 0, vertices.Length);
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];


        int xMin = (xSize / 2) * -1;
        int xMax = xSize / 2;

        int zMin = (zSize / 2) * -1;
        int zMax = zSize / 2;

        for (int i = 0, z = zMin; z <= zMax; z++)
        {
            for (int x = xMin; x <= xMax; x++)
            {
                float y = Mathf.PerlinNoise(x * .3f, z * .3f) * 2f;
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }
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
    }

    void CreateMesh()
    {
        newMesh = new GameObject("Mesh");
        newMesh.transform.position = new Vector3(0, 0, 0);
        newMesh.transform.parent = transform;
        newMesh.AddComponent<MeshFilter>();
        newMesh.AddComponent<MeshRenderer>();
        mesh = new Mesh();
        newMesh.GetComponent<MeshFilter>().mesh = mesh;
        newMesh.GetComponent<MeshRenderer>().material = snow;
        CreateShape();
        UpdateMesh();
        newMesh.AddComponent<MeshCollider>();
        newMesh.GetComponent<MeshCollider>().sharedMesh = mesh;

    }

    void UpdateMesh()
    {

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
