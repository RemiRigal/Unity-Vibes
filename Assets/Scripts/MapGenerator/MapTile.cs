using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    private MeshRenderer meshRenderer;

    private void Start() {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
    }

    public void SetMesh(Mesh mesh) {

    }
}
