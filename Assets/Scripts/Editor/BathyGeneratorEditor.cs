using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(BathyGenerator))]
public class BathyGeneratorEditor : Editor {

    BathyGenerator bathyGenerator;

    private void OnEnable() {
        bathyGenerator = (BathyGenerator)target;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (GUILayout.Button("Calculate Map") && bathyGenerator != null) {
            bathyGenerator.CalculateMap();
        }
    }
}