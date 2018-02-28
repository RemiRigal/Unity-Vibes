using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class ObjectBuilderEditor : Editor {

    MapGenerator mapGenerator;

    private void OnEnable() {
        mapGenerator = (MapGenerator)target;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (GUILayout.Button("Generate Map") && mapGenerator != null) {
            mapGenerator.GenerateMap();
        }
    }
}