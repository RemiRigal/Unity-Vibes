using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public static Dictionary<string, Color> colorDict = new Dictionary<string, Color> {
            { "RED", Color.red },
            { "GREEN", Color.green },
            { "BLUE", Color.blue },
            { "YELLOW", Color.yellow },
            { "CYAN", Color.cyan },
            { "MAGENTA", Color.magenta },
            { "GREY", Color.grey },
            { "BLACK", Color.black },
            { "WHITE", Color.white }
        };
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
        // TODO: Scene
        String typeFigure = obj.contentObj.typeFig;
        if (typeFigure == "2D") {
            CameraManager.Instance.Set2D();
        } else {
            CameraManager.Instance.Set3D();
        }
    }

    public void Animate(MainJsonObject obj) {
        float dt = obj.contentObj.dt;
        JObject token = JObject.Parse(JObject.Parse(obj.content)["frames"].ToString());
        foreach (string key in token.Properties().Select(p => p.Name).ToList()) {
            List<List<float>> array = JArray.Parse(token[key].ToString()).ToObject<List<List<float>>>();
            List<Vector3> positions = new List<Vector3>();
            List<Vector3> rotations = new List<Vector3>();
            foreach (List<float> frame in array) {
                positions.Add(new Vector3(frame[0], frame[1], frame[2]));
                rotations.Add(new Vector3(frame[3], frame[4], frame[5]));
            }
            ObjectAnimation animation = new ObjectAnimation(dt, positions, rotations);
            AnimationManager.Instance.AddAnimation(objects[int.Parse(key)], animation);
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
        o.SetColor(obj.contentObj.color);
        objects.Add(o.GetId(), o);
        SendMessage(obj);
    }

    public void DeleteObject(MainJsonObject obj) {
        if (!objects.ContainsKey(obj.contentObj.id)) {
            throw new Exception("Id not used !");
        }
        MainObject o = objects[obj.contentObj.id];
        int id = o.GetId();
        SendMessage(obj);
        objects.Remove(o.GetId());
        Destroy(o.gameObject);
        Debug.Log("Destroy " + o.type);
    }

    public void UpdateObject(MainJsonObject obj) {
        if (!objects.ContainsKey(obj.contentObj.id)) {
            throw new Exception("Id not used !");
        }
        MainObject o = objects[obj.contentObj.id];
        ContentJsonObject content = obj.contentObj;
        o.SetPosition(content.coordX, content.coordY, content.coordZ);
        o.SetEulerRotation(obj.contentObj.rotX, obj.contentObj.rotY, obj.contentObj.rotZ);
        o.SetSize(content.dimX, content.dimY, content.dimZ);
        o.SetColor(content.color);
        SendMessage(obj);
    }


    public void GetObject(MainJsonObject obj) {
        if (!objects.ContainsKey(obj.contentObj.id)) {
            throw new Exception("Id not used !");
        }
        SendMessage(obj, obj.contentObj);
    }

    private void SendMessage(MainJsonObject obj, ContentJsonObject contentObj = null) {
        MainObject o = objects[obj.contentObj.id];
        int id = o.GetId();
        SendMessageJson jsonObj = new SendMessageJson();
        ObjectType type = (ObjectType)obj.contentObj.type;
        jsonObj.action = obj.action;
        jsonObj.type = prefabs[type].ToString();
        jsonObj.msgId = obj.msgId;
        jsonObj.objId = id;
        if (contentObj != null){
            jsonObj.contentObj = contentObj;
        }
        string message = JsonConvert.SerializeObject(jsonObj); 
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
