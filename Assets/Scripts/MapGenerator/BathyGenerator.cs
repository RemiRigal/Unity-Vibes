using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BathyGenerator : MonoBehaviour {

    int csvParsedCount = 0;

    int meshSize = 255;

    float[,] xData;
    float[,] yData;
    float[,] zData;
    Vector3[,] mapVectors;
    Mesh[,] meshes;

    void ParseCSV(string path, out float[,] data) {
        string fileData = System.IO.File.ReadAllText(path);
        string[] lines = fileData.Split("\n"[0]);
        string[] lineData = (lines[0].Trim()).Split(","[0]);
        data = new float[255, 255];
        for (int i = 0; i < data.GetLength(0); i++) {
            lineData = (lines[i].Trim()).Split(","[0]);
            for (int j = 0; j < data.GetLength(1); j++) {
                if (!float.TryParse(lineData[j], out data[i, j])) {
                    data[i, j] = 0f;
                    Debug.LogError("CSV parsing error: " + lineData[j]);
                }
            }
        }
    }
	
	void GetMapVectors() {
        mapVectors = new Vector3[xData.GetLength(0), xData.GetLength(1)];
        for (int i = 0; i < mapVectors.GetLength(0); i++) {
            for (int j = 0; j < mapVectors.GetLength(1); j++) {
                mapVectors[i, j] = new Vector3(xData[i,j], yData[i,j], zData[i,j]);
            }
        }
	}

    void GenerateMesh(Vector3[,] vectors) {
        for (int xMesh = 0; xMesh < meshes.GetLength(0); xMesh++) {
            for (int yMesh = 0; yMesh < meshes.GetLength(1); yMesh++) {
                Mesh mesh = meshes[xMesh, yMesh];
                Vector3[] vertices = new Vector3[meshSize * meshSize];
                for (int i = 0; i < meshSize; i++) {
                    for (int j = 0; j < meshSize; j++) {
                        vertices[i * meshSize + j] = vectors[xMesh * meshSize + i, yMesh * meshSize + j];
                    }
                }
                List<int> triangles = new List<int>();
                for (int i = 0; i < meshSize - 1; i++) {
                    for (int j = 0; j < meshSize - 1; j++) {
                        triangles.Add(meshSize * j + i);
                        triangles.Add(meshSize * j + i + 1);
                        triangles.Add(meshSize * j + meshSize + i);

                        triangles.Add(meshSize * j + meshSize + i + 1);
                        triangles.Add(meshSize * j + meshSize + i);
                        triangles.Add(meshSize * j + i + 1);
                    }
                }
                mesh.vertices = vertices;
                mesh.SetTriangles(triangles, 0);
            }
        }
    }

    void OnCsvParsed(object data) {
        csvParsedCount++;
        if (csvParsedCount == 3) {
            Debug.Log("Generating map vectors...");
            ThreadDataRequester.RequestData(() => {
                GetMapVectors();
                return true;
            }, OnMapVectorsGenerated);
        }
    }

    void OnMapVectorsGenerated(object data) {
        int xLength = mapVectors.GetLength(0);
        int yLength = mapVectors.GetLength(1);
        int xMeshCount = xLength / meshSize;
        int yMeshCount = yLength / meshSize;
        Mesh[,] meshes = new Mesh[xMeshCount, yMeshCount];
        for (int xMesh = 0; xMesh < meshes.GetLength(0); xMesh++) {
            for (int yMesh = 0; yMesh < meshes.GetLength(1); yMesh++) {
                meshes[xMesh, yMesh] = new Mesh();
            }
        }
        Debug.Log("Generating " + (xMeshCount * yMeshCount) + " meshes...");
        ThreadDataRequester.RequestData(() => {
            GenerateMesh(mapVectors);
            return true;
        }, OnMeshesGenerated);
    }

    void OnMeshesGenerated(object data) {
        Debug.Log("Done");
    }

    public void CalculateMap() {
        Debug.Log("Parsing CSV files...");
        ThreadDataRequester.RequestData(() => {
            ParseCSV("Assets/Data/Xkrig.csv", out xData);
            return true;
        }, OnCsvParsed);
        ThreadDataRequester.RequestData(() => {
            ParseCSV("Assets/Data/Ykrig.csv", out yData);
            return true;
        }, OnCsvParsed);
        ThreadDataRequester.RequestData(() => {
            ParseCSV("Assets/Data/Zhat.csv", out zData);
            return true;
        }, OnCsvParsed);
    }

    private void Start() {
        //CalculateMap();
    }
}
