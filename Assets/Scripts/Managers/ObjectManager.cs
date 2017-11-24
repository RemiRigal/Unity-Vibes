using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour {

    private TCPServer tcpServer;

    private Dictionary<int, Object> objects;

	void Start () {
        tcpServer = new TCPServer();
        tcpServer.OnDataReceived += new HandlePacketData(OnMessageReceived);
        tcpServer.OnClientConnected += new HandleClientConnection(OnClientConnected);
	}

    void OnClientConnected() {
        Debug.Log("Client connected");
    }

    void OnMessageReceived(string message) {
        Debug.Log("Received: " + message);
    }
}
