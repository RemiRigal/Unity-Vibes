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
    public MainObject deathStar;


    private TCPServer tcpServer;
    private Dictionary<ObjectType, MainObject> prefabs = new Dictionary<ObjectType, MainObject>();
    private Dictionary<int, MainObject> objects = new Dictionary<int, MainObject>();
    private int objectCounter = 0;

    private bool first = true;

    private Queue<TCPAction> followingActions = new Queue<TCPAction>();

    void Start () {
        LoadPrefabs();
        tcpServer = new TCPServer();
        tcpServer.OnDataReceived += new HandlePacketData(OnMessageReceived);
        tcpServer.OnClientConnected += new HandleClientConnection(OnClientConnected);
	}

    void OnClientConnected() {
        Debug.Log("Client connected");
        tcpServer.Send(tcpServer.client, "Empty");
    }

    void OnMessageReceived(string message) {
        Debug.Log("Received: " + message);
        if (first) {
            Debug.Log("Connection request: " + message);
            first = false;
            ConnectionJsonObject connectionJson = JsonUtility.FromJson<ConnectionJsonObject>(message);
            tcpServer.Send(tcpServer.client, "{\"connection\": \"ok\"}");
        } else {
            try {
                MainJsonObject jsonObj = JsonUtility.FromJson<MainJsonObject>(message);
                jsonObj.contentObj = JsonUtility.FromJson<ContentJsonObject>(jsonObj.content);
                followingActions.Enqueue(new TCPAction(jsonObj));
            } catch (Exception e) {
                Debug.LogError(e);
            }
        }
        //tcpServer.Send(tcpServer.client, "Empty");
    }

    private void Update() {
        while (followingActions.Count > 0) {
            TCPAction action = followingActions.Dequeue();
            switch (action.obj.action) {
                case "Init":
                    InitFigure(action.obj);
                    break;
                case "Create":
                    CreateObject(action.obj);
                    break;
                case "Delete":
                    DeleteObject(action.obj);
                    break;
                case "Update":
                    UpdateObject(action.obj);
                    break;
                case "Get":
                    GetObject(action.obj);
                    break;
                case "CameraTracking":
                    TrackObject(action.obj);
                    break;
                default:
                    break;
            }
        }
    }

    private void InitFigure(MainJsonObject obj) {
        String typeFigure = obj.contentObj.typeFig;
        if (typeFigure == "2D") {
            CameraManager.Instance.Set2D();
        } else {
            CameraManager.Instance.Set3D();
        }
    }

    private void TrackObject(MainJsonObject obj) {
        MainObject o = objects[obj.contentObj.id];
        CameraManager.Instance.TrackObject(o);
    }

    private void CreateObject(MainJsonObject obj) {
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

    private void DeleteObject(MainJsonObject obj) {
        if (!objects.ContainsKey(obj.contentObj.id)) {
            throw new Exception("Id not used !");
        }
        MainObject o = objects[obj.contentObj.id];
        objects.Remove(o.GetId());
        Destroy(o.gameObject);
        Debug.Log("Destroy " + o.type);
    }

    private void UpdateObject(MainJsonObject obj) {
        MainObject o = objects[obj.contentObj.id];
        ContentJsonObject content = obj.contentObj;
        o.SetPosition(content.coordX, content.coordY, content.coordZ);
        o.SetEulerRotation(obj.contentObj.rotX, obj.contentObj.rotY, obj.contentObj.rotZ);
        o.SetSize(content.dimX, content.dimY, content.dimZ);
    }


    private void GetObject(MainJsonObject obj) {
        MainObject o = objects[obj.contentObj.id];
        ContentJsonObject content = obj.contentObj;
        tcpServer.Send(tcpServer.client, content.ToString());
    }

    private void LoadPrefabs() {
        prefabs.Add(plane.type, plane);
        prefabs.Add(cube.type, cube);
        prefabs.Add(sphere.type, sphere);
        prefabs.Add(capsule.type, capsule);
        prefabs.Add(cylinder.type, cylinder);
        prefabs.Add(boat.type, boat);
        prefabs.Add(submarine.type, submarine);
        prefabs.Add(deathStar.type, deathStar);
    }
}
