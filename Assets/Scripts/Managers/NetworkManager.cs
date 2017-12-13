using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour {

    private ObjectManager objectManager;
    private TCPServer tcpServer;

    void Awake () {
        objectManager = GetComponent<ObjectManager>();
        tcpServer = new TCPServer();
        tcpServer.OnDataReceived += new HandlePacketData(OnMessageReceived);
        Debug.Log("Server started");
    }

    void OnDestroy() {
        Debug.Log("Stopping server");
        tcpServer.Close();
    }

    void OnMessageReceived(string message) {
        Debug.Log("Received: " + message);

        
    }
}
