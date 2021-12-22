using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;
using UnityEditor;

public class UDPServer
{
    public UdpClient udpClient;
    public int port;
    public CustomThreadHandler UDPServerThreadHandler;
    public ServerController serverController;
    private NetArgParser parser = new NetArgParser();
    public UDPServer(int _port, ServerController _serverController)
    {
        serverController = _serverController;

        port = _port;
        udpClient = new UdpClient(port);
        UDPServerThreadHandler = new CustomThreadHandler(new Thread(() =>
        {
            Debug.Log("UDP Server started");
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, port);
            Byte[] received = udpClient.Receive(ref sender);
            string data = System.Text.Encoding.ASCII.GetString(received);
            Debug.Log("UDP Server received data: " + data + " from remote: " + sender);
            NetworkCommand cmd = parser.parse(data);
            if (!cmd.error)
            {
                try
                {
                    serverController.binds[cmd.command](cmd.args);
                }
                catch (Exception e)
                {
                    Debug.Log("Command " + cmd.command + " failed; " + e);
                }
            }
        }));

        UDPServerThreadHandler.thread.Name = "UDP Server";
        UDPServerThreadHandler.thread.IsBackground = true;
        UDPServerThreadHandler.thread.Start();
    }

    public void Send(string message, string address)
    {
        Byte[] message_bytes = System.Text.Encoding.ASCII.GetBytes(message);
        udpClient.Send(message_bytes, message_bytes.Length, address, port);
    }
}