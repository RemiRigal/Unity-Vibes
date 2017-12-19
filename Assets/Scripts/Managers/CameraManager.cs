using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    private Camera camera;
    private bool isTracking = false;
    private MainObject trackingTarget;

    public float mouseFactor = 0.1f;

    public static CameraManager Instance = null;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        camera = GetComponent<Camera>();
    }

    public void Set2D() {
        camera.orthographic = true;
        transform.position = Vector3.zero;
    }

    public void Set3D() {
        camera.orthographic = false;
    }

    public void TrackObject(MainObject obj) {
        trackingTarget = obj;
        isTracking = true;
    }

    private void Update() {
        if (Input.GetMouseButton(2)) {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            transform.position -= new Vector3(mouseX, mouseY, 0) * mouseFactor;
        }

        if (isTracking) {
            transform.LookAt(trackingTarget.transform);
        }
    }
}
