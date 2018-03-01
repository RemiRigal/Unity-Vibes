using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    [Header("Mesh Parameters")]
    public Material material;
    [Range(10f, 10000f)]
    public float size = 1000f;
    [Range(2, 128)]
    public int meshVertices = 128;

    [Header("Map Parameters")]
    [Range(0f, 10000f)]
    public float depth = 15f;
    public int perlinSeed = 888888;
    public float[] perlinFactors = new float[] { 0.01f };
    public float[] perlinInfluences = new float[] { 100f };

    private Mesh mesh;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    private void Start() {
        transform.position = new Vector3(0f, -depth, 0f);
    }

    private void OnValidate() {
        if (perlinInfluences.Length == perlinFactors.Length) {
            GenerateMap();
        }
    }

    public void GenerateMap() {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null) {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null) {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        if (mesh == null) {
            mesh = new Mesh();
            meshFilter.mesh = mesh;
        }
        meshRenderer.material = material;
        GenerateMapMesh();
        transform.position = new Vector3(0f, -depth, 0f);
    }
	
	void GenerateMapMesh() {
        mesh.Clear();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        Random.InitState(perlinSeed);
        float randX = Random.value;
        float randY = Random.value;

        float minX = - size / 2f;
        float minZ = - size / 2f;
        float delta = size / meshVertices;

        for (int x = 0; x < meshVertices; x++) {
            for (int z = 0; z < meshVertices; z++) {
                float vx = minX + delta * x;
                float vy = 0f;
                float vz = minZ + delta * z;
                for (int i = 0; i < perlinFactors.Length; i++) {
                    float factor = perlinFactors[i];
                    float influence = perlinInfluences[i];
                    vy += Mathf.PerlinNoise(randX + x * factor, randY + z * factor) * influence;
                }
                vertices.Add(new Vector3(vx, vy, vz));
            }
        }

        for (int i = 0; i < meshVertices - 1; i++) {
            for (int j = 0; j < meshVertices - 1; j++) {
                triangles.Add(meshVertices * j + i);
                triangles.Add(meshVertices * j + i + 1);
                triangles.Add(meshVertices * j + meshVertices + i);

                triangles.Add(meshVertices * j + meshVertices + i + 1);
                triangles.Add(meshVertices * j + meshVertices + i);
                triangles.Add(meshVertices * j + i + 1);
            }
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
}
