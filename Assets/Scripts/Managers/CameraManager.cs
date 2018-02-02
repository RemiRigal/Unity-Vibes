using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    private Camera camera;
    private bool isTracking = false;
    private MainObject trackingTarget;

    public float mouseMovementFactor = 10f;
    public float movementFactor = 0.2f;
    public float zoomFactor = 20f;

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

    private float mouseX;
    private float mouseY;
    private float horizontal;
    private float vertical;
    private float jump;
    private float mouseWheel;

    private void Update() {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        jump = Input.GetAxis("Jump");
        transform.position += (-transform.forward * vertical + transform.right * horizontal + transform.up * jump) * movementFactor;

        mouseWheel = Input.GetAxis("Mouse ScrollWheel");
        transform.position += transform.forward * mouseWheel * zoomFactor;

        if (isTracking) {
            transform.LookAt(trackingTarget.transform);
        } else if (Input.GetMouseButton(1)) {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
            transform.RotateAround(transform.position, Vector3.up,  mouseX * mouseMovementFactor);
            transform.RotateAround(transform.position, transform.right, - mouseY * mouseMovementFactor);
        }
    }
}
