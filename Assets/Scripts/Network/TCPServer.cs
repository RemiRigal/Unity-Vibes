using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public delegate void HandlePacketData(string message);
public delegate void HandleClientConnection();


public class TCPServer {

    public event HandlePacketData OnDataReceived;
    public event HandleClientConnection OnClientConnected;

    public ManualResetEvent allDone = new ManualResetEvent(false);

    private TcpListener server;
    private TcpClient client;
    private NetworkStream clientStream;
    private int port = 29200;
    private IPAddress address = IPAddress.Parse("192.168.43.9");
    private bool started = false;
    private StringBuilder stringBuilder = new StringBuilder();

    public TCPServer() {
        server = new TcpListener(IPAddress.Any, port);
        server.Start();

        Thread thread = new Thread(new ThreadStart(StartListening));
        started = true;
        thread.Start();

        Debug.Log("Server started");
    }

    ~TCPServer() {
        if (client != null) {
            client.Close();
            clientStream = null;
        }
        started = false;
    }

    public bool IsConnected() {
        return client != null && client.Connected;
    }

    private void StartListening() {
        try {
            client = server.AcceptTcpClient();
            clientStream = client.GetStream();
            OnClientConnected.Invoke();

            stringBuilder = new StringBuilder();
            while (started) {
                byte[] readBuffer = new byte[client.ReceiveBufferSize];
                int bytesRead = clientStream.Read(readBuffer, 0, readBuffer.Length);
                stringBuilder.Append(Encoding.ASCII.GetString(readBuffer, 0, bytesRead));
                string content = stringBuilder.ToString();

                while (content.IndexOf("\n") > -1) {
                    int end = content.IndexOf("\n");
                    string message = content.Substring(0, end);
                    content = content.Substring(end + 1);
                    OnDataReceived.Invoke(message);
                }

                stringBuilder = new StringBuilder(content);
            }
        } catch (SocketException e) {
            Console.WriteLine("SocketException: {0}", e);
        } finally {
            server.Stop();
        }
    }

    public void Send(string message) {
        byte[] writeBuffer = Encoding.ASCII.GetBytes(message);
        clientStream.Write(writeBuffer, 0, writeBuffer.Length);
    }

}


