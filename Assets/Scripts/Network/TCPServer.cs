using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public delegate void HandlePacketData(string message);
public delegate void HandleClientConnection();

public class StateObject {
    public const int BufferSize = 1024;
    public byte[] buffer = new byte[BufferSize];
    public StringBuilder sb = new StringBuilder();
}

public class TCPServer {

    public event HandlePacketData OnDataReceived;
    public event HandleClientConnection OnClientConnected;

    public ManualResetEvent allDone = new ManualResetEvent(false);

    private Socket server;
    private Socket client;
    private int port = 29200;
    private IPAddress address = IPAddress.Parse("127.0.0.1");
    private bool started = false;

    public TCPServer() {
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        IPEndPoint localEndPoint = new IPEndPoint(address, port);
        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        server.Bind(localEndPoint);

        Thread thread = new Thread(new ThreadStart(StartListening));
        started = true;
        thread.Start();
    }

    ~TCPServer() {
        started = false;
        server.Shutdown(SocketShutdown.Both);
        server.Close();
    }

    public void StartListening() {
        try {
            server.Listen(100);
            while (started) {
                allDone.Reset();
                server.BeginAccept(new AsyncCallback(AcceptCallback), server);
                allDone.WaitOne();
            }
        } catch (Exception e) {
            Console.WriteLine(e.ToString());
        }
    }

    public void AcceptCallback(IAsyncResult ar) {
        allDone.Set();
        client = server.EndAccept(ar);
        StateObject state = new StateObject();
        OnClientConnected.Invoke();
        client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
    }

    public void ReadCallback(IAsyncResult ar) {
        string content = string.Empty;
        StateObject state = (StateObject)ar.AsyncState;
        int bytesRead = client.EndReceive(ar);
        if (bytesRead > 0) {
            state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
            content = state.sb.ToString();
            if (content.IndexOf("<EOF>") > -1) {
                OnDataReceived.Invoke(content);
            } else {
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
        }
    }

    private void Send(Socket handler, string data) {
        byte[] writeBuffer = Encoding.ASCII.GetBytes(data);
        handler.BeginSend(writeBuffer, 0, writeBuffer.Length, 0, new AsyncCallback(SendCallback), handler);
    }

    private void SendCallback(IAsyncResult ar) {
        try {
            server.EndSend(ar);
        } catch (Exception e) {
            Console.WriteLine(e.ToString());
        }
    }
}


