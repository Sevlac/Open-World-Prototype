using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGnerator : MonoBehaviour
{
    Mesh mesh;
    GameObject newMesh;

    public GameObject player;

    Dictionary<Vector2, Vector3[]> myMeshes = new Dictionary<Vector2, Vector3[]>(); // Ce dictionnaire contient tout les terrains Key= positon (x,z) du centre du terrain et Value = tableau des point du terrain
    int[] triangles;

    public GameObject[] environement;

    public int xSize; //taille du terrain sur x
    public int zSize; //taille du terrain sur z

    private int xMin; //position minimum du terrain sur x
    private int xMax; //position maximum du terrain sur x

    private int zMin; //position minimum du terrain sur z
    private int zMax; //position maximum du terrain sur z

    private float middleChunkX = 0;
    private float middleChunkZ = 0;

    public float playerPosX;
    public float playerPosZ;

    public int chunks; // nombre de terrain maximum autour du personnage

    public Material land;

    private void Awake()
    {
         
         xMin = (xSize / 2) * -1;
         xMax = xSize / 2;

         zMin = (zSize / 2) * -1;
         zMax = zSize / 2;

        ChunkGenerator();

    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        playerPosX = player.transform.position.x;
        playerPosZ = player.transform.position.z;
        if (playerPosX > middleChunkX+xMax)
        {
            middleChunkX += xSize;
            ChunkGenerator();
        }
        if (playerPosX < middleChunkX - xMax)
        {
            middleChunkX -= xSize;
            ChunkGenerator();
        }
        if (playerPosZ > middleChunkZ + zMax)
        {
            middleChunkZ += zSize;
            ChunkGenerator();
        }
        if (playerPosZ < middleChunkZ - zMax)
        {
            middleChunkZ -= zSize;
            ChunkGenerator();
        }

    }

    void CreateShape()
    {
        myMeshes.Add(new Vector2(newMesh.transform.position.x, newMesh.transform.position.z), new Vector3[(xSize + 1) * (zSize + 1)]);
        GameObject environementToPlace;

        for (int i = 0, z = zMin; z <= zMax; z++)
        {
            for (int x = xMin; x <= xMax; x++)
            {
                environementToPlace = environement[Random.Range(0, environement.Length)];
                if (x > xMin && x < xMax && z > zMin && z < zMax && (x > 1 || x < -1) && (z > 1 || z < -1 ))
                {
                    if (Random.Range(0, 251) == 1)
                    {
                        var newEnvironment = Instantiate(environementToPlace, new Vector3(x + newMesh.transform.position.x, 0, z + newMesh.transform.position.z), Quaternion.identity);
                        newEnvironment.transform.parent = newMesh.transform;
                    }
                }
                myMeshes[new Vector2(newMesh.transform.position.x, newMesh.transform.position.z)][i] = new Vector3(x, 0, z);
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
        newMesh.GetComponent<MeshRenderer>().material = land;
        CreateShape();
        UpdateMesh();
        newMesh.AddComponent<MeshCollider>();
        newMesh.GetComponent<MeshCollider>().sharedMesh = mesh;

    }

    void ChunkGenerator()
    {
        float xMeshMin = middleChunkX - (xSize * (chunks + 1));
        float xMeshMax = middleChunkX + (xSize * (chunks + 1));
        float zMeshMin = middleChunkZ - (zSize * (chunks + 1));
        float zMeshMax = middleChunkZ + (zSize * (chunks + 1));
        for (float x = xMeshMin; x <= xMeshMax; x += xSize)
        {
            for (float z = zMeshMin; z <= zMeshMax; z += zSize)
            {
                if (x == xMeshMin || x == xMeshMax || z == zMeshMin || z == zMeshMax)
                {
                    if (myMeshes.ContainsKey(new Vector2(x, z)))
                    {
                        gameObject.transform.Find("Mesh" + x + "," + z).gameObject.SetActive(false);
                    }

                }
            }
        }
        for (float x = xMeshMin + xSize; x <= xMeshMax - xSize; x+=xSize)
        {
            for (float z = zMeshMin + xSize; z <= zMeshMax - xSize; z+=zSize)
            {
                if (myMeshes.ContainsKey(new Vector2(x, z)))
                {
                    gameObject.transform.Find("Mesh" + x + "," + z).gameObject.SetActive(true);
                }
                else
                {
                    CreateMesh(x, z);
                }
            }
        }
    }

    
    void UpdateMesh()
    {

        mesh.vertices = myMeshes[new Vector2(newMesh.transform.position.x, newMesh.transform.position.z)];
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
