using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public delegate void HandlePacketData(string message);

public class StateObject {
    public const int BufferSize = 1024;
    public byte[] buffer = new byte[BufferSize];
    public Socket socket;
    public StringBuilder sb = new StringBuilder();
}

public class TCPServer {

    public event HandlePacketData OnDataReceived;

    public ManualResetEvent allDone = new ManualResetEvent(false);

    private Socket server;
    private int port = 29200;
    private bool started = false;

    public TCPServer() {
        IPAddress ipAddress = IPAddress.Parse("192.168.1.25");
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        server.Bind(localEndPoint);

        Thread thread = new Thread(new ThreadStart(StartListening));
        started = true;
        thread.Start();
    }

    public void Close() {
        started = false;
        if (server != null) {
            try {
                server.Shutdown(SocketShutdown.Both);
            } catch (Exception e) {
                Debug.LogWarning("Server did not shutdown properly: " + e.Message);
            }
            server.Close();
        }
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
        Socket client = server.EndAccept(ar);
        StateObject state = new StateObject();
        state.socket = client;
        client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
    }

    public void ReadCallback(IAsyncResult ar) {
        string content = string.Empty;
        StateObject state = (StateObject)ar.AsyncState;
        Socket client = state.socket;
        int bytesRead = client.EndReceive(ar);
        if (bytesRead > 0) {
            state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
            content = state.sb.ToString();
            if (content.IndexOf("\n") > -1) {
                string[] splittedContent = content.Split(new string[] { "\n" }, StringSplitOptions.None);
                for (int i = 0; i < splittedContent.Length - 1; i++) {
                    OnDataReceived.Invoke(splittedContent[i]);
                }
                state.sb = new StringBuilder();
                state.sb.Append(splittedContent[splittedContent.Length - 1]);
            }
            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
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


