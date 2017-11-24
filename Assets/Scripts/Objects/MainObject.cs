using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MainObject : MonoBehaviour {

    public readonly bool is3D;
    public int id = -1;

    private void Start() {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public void SetPosition(Vector3 newPosition) {
        transform.position = newPosition;
    }

    public void SetEulerRotation(float rx, float ry, float rz) {
        transform.rotation = Quaternion.Euler(rx, ry, rz);
    }

    public void SetSize(float sx, float sy, float sz) {
        SetSize(new Vector3(sx, sy, sz));
    }

    public void SetSize(Vector3 newSize) {
        transform.localScale = newSize;
    }
}

public enum ObjectType3D : int {
    Cube, Sphere, Capsule, Cylinder
}

public enum ObjectType2D : int {
    Plane
}
