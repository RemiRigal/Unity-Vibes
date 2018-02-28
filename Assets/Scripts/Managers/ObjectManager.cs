using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ObjectManager : MonoBehaviour {

    public MainObject plane;
    public MainObject cube;
    public MainObject sphere;
    public MainObject capsule;
    public MainObject cylinder;
    public MainObject boat;
    public MainObject submarine;
    public MainObject galleon;

    public static ObjectManager Instance = null;

    private Dictionary<ObjectType, MainObject> prefabs = new Dictionary<ObjectType, MainObject>();
    private Dictionary<int, MainObject> objects = new Dictionary<int, MainObject>();
    private int objectCounter = 0;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start () {
        LoadPrefabs();
	}

    public void InitFigure(MainJsonObject obj) {
        String typeFigure = obj.contentObj.typeFig;
        if (typeFigure == "2D") {
            CameraManager.Instance.Set2D();
        } else {
            CameraManager.Instance.Set3D();
        }
    }

    public void TrackObject(MainJsonObject obj) {
        MainObject o = objects[obj.contentObj.id];
        CameraManager.Instance.TrackObject(o);
    }

    public void CreateObject(MainJsonObject obj) {
        if (objects.ContainsKey(obj.contentObj.id)) {
            throw new Exception("Id already used");
        }
        ObjectType type = (ObjectType)obj.contentObj.type;
        MainObject prefab = prefabs[type];
        Vector3 position = new Vector3(obj.contentObj.coordX, obj.contentObj.coordY, obj.contentObj.coordZ);
        GameObject go = Instantiate(prefab.gameObject, position, Quaternion.identity);
        MainObject o = go.GetComponent<Object3D>();
        o.SetId(obj.contentObj.id);
        o.SetEulerRotation(obj.contentObj.rotX, obj.contentObj.rotY, obj.contentObj.rotZ);
        o.SetSize(obj.contentObj.dimX, obj.contentObj.dimY, obj.contentObj.dimZ);
        objects.Add(o.GetId(), o);
        Debug.Log("Created " + o.type);
    }

    public void DeleteObject(MainJsonObject obj) {
        if (!objects.ContainsKey(obj.contentObj.id)) {
            throw new Exception("Id not used !");
        }
        MainObject o = objects[obj.contentObj.id];
        objects.Remove(o.GetId());
        Destroy(o.gameObject);
        Debug.Log("Destroy " + o.type);
    }

    public void UpdateObject(MainJsonObject obj) {
        MainObject o = objects[obj.contentObj.id];
        ContentJsonObject content = obj.contentObj;
        o.SetPosition(content.coordX, content.coordY, content.coordZ);
        o.SetEulerRotation(obj.contentObj.rotX, obj.contentObj.rotY, obj.contentObj.rotZ);
        o.SetSize(content.dimX, content.dimY, content.dimZ);
        //o.SetColor();
    }


    public void GetObject(MainJsonObject obj) {
        MainObject o = objects[obj.contentObj.id];
        ContentJsonObject content = obj.contentObj;
        String message = JsonUtility.ToJson(content);
        NetworkManager.Instance.Send(message);
    }

    private void LoadPrefabs() {
        prefabs.Add(plane.type, plane);
        prefabs.Add(cube.type, cube);
        prefabs.Add(sphere.type, sphere);
        prefabs.Add(capsule.type, capsule);
        prefabs.Add(cylinder.type, cylinder);
        prefabs.Add(boat.type, boat);
        prefabs.Add(submarine.type, submarine);
        prefabs.Add(galleon.type, galleon);
    }
}
