using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour {

    private TCPServer tcpServer;
    private Queue<TCPAction> followingActions = new Queue<TCPAction>();

    public static NetworkManager Instance = null;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        tcpServer = new TCPServer();
        tcpServer.OnDataReceived += new HandlePacketData(OnMessageReceived);
        tcpServer.OnClientConnected += new HandleClientConnection(OnClientConnected);
    }

    public void Send(string message) {
        if (tcpServer.IsConnected()) {
            tcpServer.Send(message);
        }
    }

    void OnClientConnected() {
        Debug.Log("Client connected");
    }

    void OnMessageReceived(string message) {
        MainJsonObject jsonObj = JsonConvert.DeserializeObject<MainJsonObject>(message);
        jsonObj.contentObj = JsonConvert.DeserializeObject<ContentJsonObject>(jsonObj.content);  
        followingActions.Enqueue(new TCPAction(jsonObj));
    }

    private void Update() {
        while (followingActions.Count > 0) {
            TCPAction action = followingActions.Dequeue();
            switch (action.obj.action) {
                case "Init":
                    ObjectManager.Instance.InitFigure(action.obj);
                    break;
                case "Create":
                    ObjectManager.Instance.CreateObject(action.obj);
                    break;
                case "Delete":
                    ObjectManager.Instance.DeleteObject(action.obj);
                    break;
                case "Update":
                    ObjectManager.Instance.UpdateObject(action.obj);
                    break;
                case "Get":
                    ObjectManager.Instance.GetObject(action.obj);
                    break;
                case "CameraTracking":
                    ObjectManager.Instance.TrackObject(action.obj);
                    break;
            }
        }
    }
}
