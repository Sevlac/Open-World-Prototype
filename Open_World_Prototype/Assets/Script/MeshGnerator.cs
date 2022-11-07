using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGnerator : MonoBehaviour
{
    Mesh mesh;
    GameObject newMesh;

    Dictionary<Vector2, Vector3[]> myMeshes = new Dictionary<Vector2, Vector3[]>(); // Ce dictionnaire contient tout les terrains Key= positon (x,z) du centre du terrain et Value = tableau des point du terrain
    int[] triangles;

    public int xSize; //taille du terrain sur x
    public int zSize; //taille du terrain sur z

    private int xMin; //position minimum du terrain sur x
    private int xMax; //position maximum du terrain sur x

    private int zMin; //position minimum du terrain sur z
    private int zMax; //position maximum du terrain sur z

    private float centerX = 0;
    private float centerZ = 0;

    public int chunks; // nombre de terrain maximum autour du personnage

    public Material snow;

    private void Awake()
    {

         xMin = (xSize / 2) * -1;
         xMax = xSize / 2;

         zMin = (zSize / 2) * -1;
         zMax = zSize / 2;

        CreateMesh(0,0);

    }
    void Start()
    {
        ChunkGenerator();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateShape()
    {
        myMeshes.Add(new Vector2(newMesh.transform.position.x, newMesh.transform.position.z), new Vector3[(xSize + 1) * (zSize + 1)]);

        for (int i = 0, z = zMin; z <= zMax; z++)
        {
            for (int x = xMin; x <= xMax; x++)
            {
                float y = CalculateY(x, z, i);
                myMeshes[new Vector2(newMesh.transform.position.x, newMesh.transform.position.z)][i] = new Vector3(x, y, z);
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

    void CreateMesh(float x, float z)
    {
        newMesh = new GameObject("Mesh" + x + "," + z);
        newMesh.transform.position = new Vector3(x, 0, z);
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

    void ChunkGenerator()
    {

        for (float x = centerX - (xSize * chunks); x <= (xSize * chunks); x+=xSize)
        {
            for (float z = centerZ - (zSize * chunks); z <= (zSize * chunks); z+=zSize)
            {
                if (x != centerX || z != centerZ)
                {
                    CreateMesh(x,z);

                }
            }
        }
    }

    private float CalculateY(int x, int z, int i)
    {
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

        float y = Mathf.PerlinNoise(x * Random.Range(.1f, .5f), z * Random.Range(.1f, .5f)) * 2f;

        if (x == xMin && leftMesh)
        {
            y = myMeshes[new Vector2(newMeshX - xSize, newMeshZ)][i + xSize].y;
        }
        if (z == zMax && topMesh)
        {
            y = myMeshes[new Vector2(newMeshX, newMeshZ + zSize)][i - (xSize * (zSize + 1))].y;
        }
        if (x == xMax && rightMesh)
        {
            y = myMeshes[new Vector2(newMeshX + xSize, newMeshZ)][i - xSize].y;
        }
        if (z == zMin && bottomMesh)
        {
            y = myMeshes[new Vector2(newMeshX, newMeshZ - zSize)][i + (xSize * (zSize + 1))].y;
        }
        if (x == xMin && z == zMax && !leftMesh && !topMesh && topLeftMesh)
        {
            y = myMeshes[new Vector2(newMeshX - xSize, newMeshZ + zSize)][i - (xSize * (zSize + 1)) + xSize].y;
        }
        if (x == xMax && z == zMax && !rightMesh && !topMesh && topRightMesh)
        {
            y = myMeshes[new Vector2(newMeshX + xSize, newMeshZ + zSize)][i - (xSize * (zSize + 1)) - xSize].y;
        }
        if (x == xMax && z == zMin && !rightMesh && !bottomMesh && bottomRightMesh)
        {
            y = myMeshes[new Vector2(newMeshX + xSize, newMeshZ - zSize)][i + (xSize * (zSize + 1)) - xSize].y;
        }
        if (x == xMin && z == zMin && !rightMesh && !bottomMesh && bottomLeftMesh)
        {
            y = myMeshes[new Vector2(newMeshX - xSize, newMeshZ - zSize)][i + (xSize * (zSize + 1)) + xSize].y;
        }
        return y;
    }
    void UpdateMesh()
    {

        mesh.vertices = myMeshes[new Vector2(newMesh.transform.position.x, newMesh.transform.position.z)];
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
